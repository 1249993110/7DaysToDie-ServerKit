using Autofac;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.Data.Repositories;
using SdtdServerKit.WebSockets;
using System.Text;
using UnityEngine;

namespace SdtdServerKit.Hooks
{
    internal static class ModEventHook
    {
        #region Broadcast

        private static void Broadcast(ModEventType modEventType)
        {
            try
            {                
                string json = JsonConvert.SerializeObject(new WebSocketMessage(modEventType), ModApi.JsonSerializerSettings);
                ModApi.WebSocketSessionManager.BroadcastAsync(json, null);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in ModEventHook.Broadcast");
            }
        }

        private static void Broadcast<TData>(ModEventType modEventType, TData data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(new WebSocketMessage<TData>(modEventType, data), ModApi.JsonSerializerSettings);
                ModApi.WebSocketSessionManager.BroadcastAsync(json, null);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in ModEventHook.Broadcast");
            }
        }
        #endregion

        /// <summary>
        /// Runs once when the server is ready for interaction and GameManager.Instance.World is set
        /// </summary>
        public static void OnGameAwake()
        {
            if (ModApi.WebSocketSessionManager.Count == 0) 
                return;
            Broadcast(ModEventType.GameAwake);
        }

        /// <summary>
        /// Runs once when the server is ready for players to join
        /// </summary>
        public static void OnGameStartDone()
        {
            if (ModApi.WebSocketSessionManager.Count == 0) 
                return;
            Broadcast(ModEventType.GameStartDone);
        }

        /// <summary>
        /// <para>Runs any time the game executes an update (which is very often).</para>
        /// <para>Place any tasks that you want to process in the main thread here with an execution rate limiter (such as creating entities via the entity factory)</para>
        /// </summary>
        public static void OnGameUpdate()
        {
            if (ModApi.WebSocketSessionManager.Count == 0) 
                return;
            Broadcast(ModEventType.GameUpdate);
        }

        /// <summary>
        /// Runs once when the server is about to shut down
        /// </summary>
        public static void OnGameShutdown()
        {
            if (ModApi.WebSocketSessionManager.Count == 0) 
                return;
            Broadcast(ModEventType.GameShutdown);
        }

        /// <summary>
        /// Runs each time a chunk has it's colors re-calculated. For example this is used to generate the images for allocs game map mod
        /// </summary>
        /// <param name="chunk"></param>
        public static void OnCalcChunkColorsDone(Chunk chunk)
        {
            if (ModApi.WebSocketSessionManager.Count == 0) 
                return;
            Broadcast(ModEventType.CalcChunkColorsDone);
        }

        /// <summary>
        /// <para>Return true to pass the message on to the next mod, or if no other mods then it will output to chat. </para>
        /// <para>Return false to prevent the message from being passed on or output to chat</para>
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="eChatType"></param>
        /// <param name="senderId"></param>
        /// <param name="message"></param>
        /// <param name="mainName"></param>
        /// <param name="localizeMain"></param>
        /// <param name="recipientEntityIds"></param>
        /// <returns></returns>
        public static bool OnChatMessage(ClientInfo? clientInfo, EChatType eChatType, int senderId, string message,
            string mainName, bool localizeMain, List<int> recipientEntityIds)
        {
            var playerId = ConnectionManager.Instance.Clients.ForEntityId(senderId).InternalId.CombinedString;
            var chatMessage = new ChatMessage()
            {
                ChatType = (ChatType)eChatType,
                EntityId = senderId,
                PlayerId = playerId,
                Message = message,
                SenderName = clientInfo?.playerName ?? (localizeMain ? Localization.Get(mainName) : mainName),
            };

            try
            {
                var chatRecordRepository = ModApi.ServiceContainer.Resolve<IChatRecordRepository>();
                chatRecordRepository.Insert(new T_ChatRecord()
                {
                    CreatedAt = DateTime.Now,
                    ChatType = chatMessage.ChatType,
                    PlayerId = chatMessage.PlayerId,
                    Message = chatMessage.Message,
                    SenderName = chatMessage.SenderName,
                });
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in PersistentManager.OnChatMessage");
            }

            if (ModApi.WebSocketSessionManager.Count == 0)
                return true;
            Broadcast(ModEventType.ChatMessage, chatMessage);

            return true;
        }

        /// <summary>
        /// Runs when entity has been killed
        /// </summary>
        /// <param name="killedEntity"></param>
        /// <param name="entityThatKilledMe"></param>
        public static void OnEntityKilled(Entity killedEntity, Entity entityThatKilledMe)
        {
            if (ModApi.WebSocketSessionManager.Count == 0) 
                return;
            if (killedEntity != null
               && entityThatKilledMe != null
               && entityThatKilledMe is EntityPlayer entityPlayer
               && entityThatKilledMe.IsClientControlled())
            {
                int killerEntityId = ConnectionManager.Instance.Clients.ForEntityId(entityPlayer.entityId).entityId;

                EntityInfo entity;

                if (killedEntity is EntityPlayer diedPlayer && killedEntity.IsClientControlled())
                {
                    entity = new EntityInfo()
                    {
                        EntityId = diedPlayer.entityId,
                        EntityName = diedPlayer.EntityName,
                        Position = diedPlayer.position.ToPosition(),
                        EntityType = (Shared.Models.EntityType)killedEntity.entityType
                    };
                }
                else if (killedEntity is EntityAlive diedEntity && killedEntity.IsClientControlled() == false)
                {
                    entity = new EntityInfo()
                    {
                        EntityId = diedEntity.entityId,
                        EntityName = diedEntity.EntityName,
                        Position = diedEntity.position.ToPosition(),
                        EntityType = (Shared.Models.EntityType)killedEntity.entityType
                    };
                }
                else
                {
                    return;
                }

                Broadcast(ModEventType.EntityKilled, new KilledEntity() { DeadEntity = entity, KillerEntityId = killerEntityId });
            }
        }

        /// <summary>
        /// Runs when entity spawned
        /// </summary>
        /// <param name="entity"></param>
        public static void OnEntitySpawned(EntityInfo entity)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            Broadcast(ModEventType.EntitySpawned, entity);
        }

        /// <summary>
        /// Runs when LogCallback has be called
        /// </summary>
        /// <param name="message"></param>
        /// <param name="trace"></param>
        /// <param name="type"></param>
        public static void OnLogCallback(string message, string trace, LogType type)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            var logEntry = new LogEntry()
            {
                Message = message,
                LogLevel = (LogLevel)type,
            };
            Broadcast(ModEventType.LogCallback, logEntry);
        }

        /// <summary>
        /// Runs on each player disconnect
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="shutdown"></param>
        public static void OnPlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            Broadcast(ModEventType.PlayerDisconnected, clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// Runs on initial connection from a player, _cInfo is usually null at this point
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="compatibilityVersion"></param>
        /// <param name="stringBuilder"></param>
        public static bool OnPlayerLogin(ClientInfo clientInfo, string compatibilityVersion, StringBuilder stringBuilder)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return true;
            Broadcast(ModEventType.PlayerLogin, clientInfo.ToOnlinePlayer());
            return true;
        }

        /// <summary>
        /// Runs each time a player spawns, including on login, respawn from death, and teleport
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="respawnType"></param>
        /// <param name="position"></param>
        public static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            var spawnedPlayer = clientInfo.ToSpawnedPlayer(respawnType, position);
            Broadcast(ModEventType.PlayerSpawnedInWorld, spawnedPlayer);
        }

        /// <summary>
        /// Runs just before a player is spawned int the world
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="chunkViewDim"></param>
        /// <param name="playerProfile"></param>
        public static void OnPlayerSpawning(ClientInfo clientInfo, int chunkViewDim, PlayerProfile playerProfile)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            Broadcast(ModEventType.PlayerSpawning, clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// <para>Runs each time a player file is saved from the client to the server</para>
        /// <para>this will usually run about every 30 seconds per player as well as triggered updates such as dying</para>
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="pdf"></param>
        public static void OnSavePlayerData(ClientInfo clientInfo, PlayerDataFile pdf)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            //Broadcast(ModEventType.SavePlayerData, clientInfo.ToOnlinePlayerDetails());
            Broadcast(ModEventType.SavePlayerData, clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// Runs on game sky changed.
        /// </summary>
        /// <param name="skyChanged"></param>
        public static void OnSkyChanged(SkyChanged skyChanged)
        {
            if (ModApi.WebSocketSessionManager.Count == 0)
                return;
            Broadcast(ModEventType.SkyChanged, skyChanged);
        }
    }
}