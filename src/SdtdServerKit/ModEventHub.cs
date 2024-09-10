using SdtdServerKit.Hooks;
using SdtdServerKit.Managers;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;
using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit
{
    /// <summary>
    /// Provide a hub for various events in the mod.
    /// </summary>
    public static class ModEventHub
    {
        #region Events
        /// <summary>
        /// Event that is triggered when a log entry is received.
        /// </summary>
        public static event Action<LogEntry>? LogCallback;

        /// <summary>
        /// Event that is triggered when the game is awake and ready for interaction.
        /// </summary>
        public static event Action? GameAwake;

        /// <summary>
        /// Event that is triggered when the game has finished starting and players can join.
        /// </summary>
        public static event Action? GameStartDone;

        /// <summary>
        /// Event that is triggered on each game update.
        /// </summary>
        public static event Action? GameUpdate;

        /// <summary>
        /// Event that is triggered when the game is about to shut down.
        /// </summary>
        public static event Action? GameShutdown;

        //public static event Action CalcChunkColorsDone;

        /// <summary>
        /// Event that is triggered when a chat message is received.
        /// </summary>
        public static event Action<ChatMessage>? ChatMessage;

        /// <summary>
        /// Event that is triggered when an entity is killed.
        /// </summary>
        public static event Action<KilledEntity>? EntityKilled;

        /// <summary>
        /// Event that is triggered when an entity is spawned.
        /// </summary>
        public static event Action? EntitySpawned;

        /// <summary>
        /// Event that is triggered when a player disconnects.
        /// </summary>
        public static event Action<ManagedPlayer>? PlayerDisconnected;

        /// <summary>
        /// Event that is triggered when a player logs in.
        /// </summary>
        public static event Action<PlayerBase>? PlayerLogin;

        /// <summary>
        /// Event that is triggered when a player is spawned in the world.
        /// </summary>
        public static event Action<SpawnedPlayer>? PlayerSpawnedInWorld;

        /// <summary>
        /// Event that is triggered when a player is about to spawn in the world.
        /// </summary>
        public static event Action<ManagedPlayer>? PlayerSpawning;

        /// <summary>
        /// Event that is triggered when player data is saved.
        /// </summary>
        public static event Action<ManagedPlayer>? SavePlayerData;

        /// <summary>
        /// Event that is triggered when the sky changes.
        /// </summary>
        public static event Action<SkyChanged>? SkyChanged;
        #endregion

        /// <summary>
        /// Runs when LogCallback has been called.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="trace">The stack trace.</param>
        /// <param name="type">The log type.</param>
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
        /// Runs once when the server is ready for interaction and GameManager.Instance.World is set.
        /// </summary>
        public static void OnGameAwake()
        {
            GameAwake?.Invoke();
        }

        /// <summary>
        /// Runs once when the server is ready for players to join.
        /// </summary>
        public static void OnGameStartDone()
        {
            GameStartDone?.Invoke();
        }

        ///// <summary>
        ///// Runs any time the game executes an update (which is very often).
        ///// Place any tasks that you want to process in the main thread here with an execution rate limiter (such as creating entities via the entity factory).
        ///// </summary>
        //public static void OnGameUpdate()
        //{
        //    GameUpdate?.Invoke();
        //}

        /// <summary>
        /// Runs once when the server is about to shut down.
        /// </summary>
        public static void OnGameShutdown()
        {
            GameShutdown?.Invoke();
        }

        ///// <summary>
        ///// Runs each time a chunk has its colors re-calculated. For example, this is used to generate the images for Allocs Game Map mod.
        ///// </summary>
        ///// <param name="chunk">The chunk.</param>
        //public static void OnCalcChunkColorsDone(Chunk chunk)
        //{
        //    CalcChunkColorsDone?.Invoke(chunk);
        //}

        /// <summary>
        /// Handles a chat message.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="eChatType">The chat type.</param>
        /// <param name="senderEntityId">The sender entity ID.</param>
        /// <param name="message">The chat message.</param>
        /// <param name="mainName">The main name.</param>
        /// <param name="recipientEntityIds">The recipient entity IDs.</param>
        /// <returns>True to pass the message on to the next mod or output to chat, false to prevent the message from being passed on or output to chat.</returns>
        public static bool OnChatMessage(ClientInfo? clientInfo, EChatType eChatType, int senderEntityId, string message, string mainName, List<int> recipientEntityIds)
        {
            if (ChatMessage == null)
            {
                return true;
            }

            string senderName;
            if (clientInfo == ModApi.CmdExecuteDelegate) 
            {
                string[] parts = message.Split(new string[] { ": " }, 2, StringSplitOptions.None);
                senderName = parts[0];
                message = parts[1];
            }
            else if(senderEntityId == -1)
            {
                senderName = Localization.Get("xuiChatServer", false);
            }
            else
            {
                senderName = mainName ?? Common.NonPlayer;
            }

            var chatMessage = new ChatMessage()
            {
                EntityId = senderEntityId,
                PlayerId = clientInfo?.InternalId.CombinedString,
                ChatType = (ChatType)eChatType,
                Message = message,
                SenderName = senderName,
                CreatedAt = DateTime.Now,
            };

            Task.Run(async () =>
            {
                await ChatMessageHook.OnChatMessage(chatMessage);
            });
            Task.Run(async () =>
            {
                await PersistentManager.SaveChatMessage(chatMessage);
            });

            ChatMessage.Invoke(chatMessage);

            return true;
        }

        /// <summary>
        /// Runs when an entity is killed.
        /// </summary>
        /// <param name="victim">The killed entity.</param>
        /// <param name="killer">The entity that killed the entity.</param>
        public static void OnEntityKilled(Entity victim, Entity killer)
        {
            if (EntityKilled == null)
            {
                return;
            }

            if (victim != null
               && killer != null
               && killer is EntityPlayer entityPlayer
               && killer.IsClientControlled())
            {
                EntityInfo deadEntityInfo;
                if (victim is EntityPlayer diedPlayer && victim.IsClientControlled())
                {
                    deadEntityInfo = new EntityInfo()
                    {
                        EntityId = diedPlayer.entityId,
                        EntityName = diedPlayer.EntityName,
                        Position = diedPlayer.position.ToPosition(),
                        EntityType = Models.EntityType.OfflinePlayer
                    };
                }
                else if (victim is EntityAlive diedEntity && victim.IsClientControlled() == false)
                {
                    deadEntityInfo = new EntityInfo()
                    {
                        EntityId = diedEntity.entityId,
                        EntityName = diedEntity.EntityName,
                        Position = diedEntity.position.ToPosition(),
                        EntityType = (Models.EntityType)diedEntity.entityType
                    };
                }
                else
                {
                    return;
                }

                EntityKilled.Invoke(new KilledEntity() { DeadEntity = deadEntityInfo, KillerEntityId = entityPlayer.entityId });
            }
        }

        /// <summary>
        /// Runs when an entity is spawned.
        /// </summary>
        /// <param name="entity">The spawned entity.</param>
        public static void OnEntitySpawned(EntityInfo entity)
        {
            EntitySpawned?.Invoke();
        }

        /// <summary>
        /// Runs on each player disconnect.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="shutdown">Indicates if the server is shutting down.</param>
        public static void OnPlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            if (LivePlayerManager.TryGetByEntityId(clientInfo.entityId, out var managedPlayer))
            {
                LivePlayerManager.Remove(clientInfo);
                PlayerDisconnected?.Invoke(managedPlayer!);
                return;
            }
            else
            {
                CustomLogger.Warn($"Player disconnected but could not find online player: {clientInfo.playerName} ({clientInfo.InternalId.CombinedString})");
            }
        }

        /// <summary>
        /// Runs on initial connection from a player. The client info is usually null at this point.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="compatibilityVersion">The compatibility version.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <returns>True to allow the player to log in, false otherwise.</returns>
        public static bool OnPlayerLogin(ClientInfo clientInfo, string compatibilityVersion, StringBuilder stringBuilder)
        {
            var playerBase = new PlayerBase(
                clientInfo.InternalId.CombinedString,
                clientInfo.playerName,
                clientInfo.entityId,
                clientInfo.PlatformId.CombinedString);
            PlayerLogin?.Invoke(playerBase);
            return true;
        }

        /// <summary>
        /// Runs each time a player spawns, including on login, respawn from death, and teleport.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="respawnType">The respawn type.</param>
        /// <param name="position">The position.</param>
        public static void OnPlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            var spawnedPlayer = new SpawnedPlayer()
            {
                EntityId = clientInfo.entityId,
                PlayerId = clientInfo.InternalId.CombinedString,
                PlayerName = clientInfo.playerName,
                PlatformId = clientInfo.PlatformId.CombinedString,
                RespawnType = (Models.RespawnType)respawnType,
                Position = position.ToPosition(),
            };
            PlayerSpawnedInWorld?.Invoke(spawnedPlayer);
        }

        /// <summary>
        /// Runs just before a player is spawned in the world.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="chunkViewDim">The chunk view dimension.</param>
        /// <param name="playerProfile">The player profile.</param>
        public static void OnPlayerSpawning(ClientInfo clientInfo, int chunkViewDim, PlayerProfile playerProfile)
        {
            var managedPlayer = LivePlayerManager.Add(clientInfo);
            PlayerSpawning?.Invoke(managedPlayer);
        }

        /// <summary>
        /// Runs each time a player file is saved from the client to the server.
        /// This will usually run about every 30 seconds per player as well as triggered updates such as dying.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="pdf">The player data file.</param>
        public static void OnSavePlayerData(ClientInfo clientInfo, PlayerDataFile pdf)
        {
            if (LivePlayerManager.TryGetByEntityId(clientInfo.entityId, out var managedPlayer))
            {
                SavePlayerData?.Invoke(managedPlayer!);
                return;
            }
            else
            {
                CustomLogger.Warn($"Save player data but could not find online player: {clientInfo.playerName} ({clientInfo.InternalId.CombinedString})");
            }
        }

        /// <summary>
        /// Runs when the game sky changes.
        /// </summary>
        /// <param name="skyChanged">The sky changed event arguments.</param>
        public static void OnSkyChanged(SkyChanged skyChanged)
        {
            SkyChanged?.Invoke(skyChanged);
        }
    }
}