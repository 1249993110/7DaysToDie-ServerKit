using System.Text;
using UnityEngine;

namespace SdtdServerKit.Hooks
{
    public static class ModEventHook
    {
        #region Events
        public static event Action<LogEntry> LogCallback;
        public static event Action GameAwake;
        public static event Action GameStartDone;
        public static event Action GameUpdate;
        public static event Action GameShutdown;
        //public static event Action CalcChunkColorsDone;
        public static event Action<ChatMessage> ChatMessage;
        public static event Action<KilledEntity> EntityKilled;
        public static event Action EntitySpawned;
        public static event Action<OnlinePlayer> PlayerDisconnected;
        public static event Action<OnlinePlayer> PlayerLogin;
        public static event Action<SpawnedPlayer> PlayerSpawnedInWorld;
        public static event Action<OnlinePlayer> PlayerSpawning;
        public static event Action<OnlinePlayer> SavePlayerData;
        /// <summary>
        /// 天空变化事件
        /// </summary>
        public static event Action<SkyChanged>? SkyChanged;

        #endregion

        /// <summary>
        /// Runs when LogCallback has be called
        /// </summary>
        /// <param name="message"></param>
        /// <param name="trace"></param>
        /// <param name="type"></param>
        public static void OnLogCallback(string message, string trace, LogType type)
        {
            var logEntry = new LogEntry()
            {
                Message = message,
                LogLevel = (LogLevel)type,
            };
            LogCallback?.Invoke(logEntry);
        }

        /// <summary>
        /// Runs once when the server is ready for interaction and GameManager.Instance.World is set
        /// </summary>
        public static void OnGameAwake()
        {
            GameAwake?.Invoke();
        }

        /// <summary>
        /// Runs once when the server is ready for players to join
        /// </summary>
        public static void OnGameStartDone()
        {
            GameStartDone?.Invoke();
        }

        /// <summary>
        /// <para>Runs any time the game executes an update (which is very often).</para>
        /// <para>Place any tasks that you want to process in the main thread here with an execution rate limiter (such as creating entities via the entity factory)</para>
        /// </summary>
        public static void OnGameUpdate()
        {
            GameUpdate?.Invoke();
        }

        /// <summary>
        /// Runs once when the server is about to shut down
        /// </summary>
        public static void OnGameShutdown()
        {
            GameShutdown?.Invoke();
        }

        ///// <summary>
        ///// Runs each time a chunk has it's colors re-calculated. For example this is used to generate the images for allocs game map mod
        ///// </summary>
        ///// <param name="chunk"></param>
        //public static void OnCalcChunkColorsDone(Chunk chunk)
        //{
        //    CalcChunkColorsDone?.Invoke(chunk);
        //}

        /// <summary>
        /// <para>Return true to pass the message on to the next mod, or if no other mods then it will output to chat. </para>
        /// <para>Return false to prevent the message from being passed on or output to chat</para>
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="eChatType"></param>
        /// <param name="senderId"></param>
        /// <param name="message"></param>
        /// <param name="mainName"></param>
        /// <param name="recipientEntityIds"></param>
        /// <returns></returns>
        public static bool OnChatMessage(ClientInfo? clientInfo, EChatType eChatType, int senderId, string message,
            string mainName, List<int> recipientEntityIds)
        {
            if(ChatMessage == null)
            {
                return true;
            }

            string? playerId = null;
            int entityId = clientInfo == null ? senderId : clientInfo.entityId;
            if(GameManager.Instance.World.Players.dict.TryGetValue(entityId, out EntityPlayer player))
            {
                playerId = ConnectionManager.Instance.Clients.ForEntityId(entityId)?.InternalId.CombinedString;
            }
        
            var chatMessage = new ChatMessage()
            {
                ChatType = (ChatType)eChatType,
                EntityId = entityId,
                PlayerId = playerId,
                Message = message,
                //SenderName = clientInfo?.playerName ?? (localizeMain ? Localization.Get(mainName) : mainName),
                SenderName = clientInfo?.playerName ?? Localization.Get(mainName),
                CreatedAt = DateTime.Now,
            };

            ChatMessageHook.OnChatMessage(chatMessage);
            ChatMessage.Invoke(chatMessage);

            return true;
        }

        /// <summary>
        /// Runs when entity has been killed
        /// </summary>
        /// <param name="killedEntity"></param>
        /// <param name="entityThatKilledMe"></param>
        public static void OnEntityKilled(Entity killedEntity, Entity entityThatKilledMe)
        {
            if(EntityKilled == null)
            {
                return;
            }

            if (killedEntity != null
               && entityThatKilledMe != null
               && entityThatKilledMe is EntityPlayer entityPlayer
               && entityThatKilledMe.IsClientControlled())
            {
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

                //int killerEntityId = ConnectionManager.Instance.Clients.ForEntityId(entityPlayer.entityId).entityId;
                EntityKilled.Invoke(new KilledEntity() { DeadEntity = entity, KillerEntityId = entityPlayer.entityId });
            }
        }

        /// <summary>
        /// Runs when entity spawned
        /// </summary>
        /// <param name="entity"></param>
        public static void OnEntitySpawned(EntityInfo entity)
        {
            EntitySpawned?.Invoke();
        }

        /// <summary>
        /// Runs on each player disconnect
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="shutdown"></param>
        public static void OnPlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            PlayerDisconnected?.Invoke(clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// Runs on initial connection from a player, _cInfo is usually null at this point
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="compatibilityVersion"></param>
        /// <param name="stringBuilder"></param>
        public static bool OnPlayerLogin(ClientInfo clientInfo, string compatibilityVersion, StringBuilder stringBuilder)
        {
            PlayerLogin?.Invoke(clientInfo.ToOnlinePlayer());
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
            PlayerSpawnedInWorld?.Invoke(clientInfo.ToSpawnedPlayer(respawnType, position));
        }

        /// <summary>
        /// Runs just before a player is spawned int the world
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="chunkViewDim"></param>
        /// <param name="playerProfile"></param>
        public static void OnPlayerSpawning(ClientInfo clientInfo, int chunkViewDim, PlayerProfile playerProfile)
        {
            PlayerSpawning?.Invoke(clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// <para>Runs each time a player file is saved from the client to the server</para>
        /// <para>this will usually run about every 30 seconds per player as well as triggered updates such as dying</para>
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="pdf"></param>
        public static void OnSavePlayerData(ClientInfo clientInfo, PlayerDataFile pdf)
        {
            SavePlayerData?.Invoke(clientInfo.ToOnlinePlayer());
        }

        /// <summary>
        /// Runs on game sky changed.
        /// </summary>
        /// <param name="skyChanged"></param>
        public static void OnSkyChanged(SkyChanged skyChanged)
        {
            SkyChanged?.Invoke(skyChanged);
        }

    }
}