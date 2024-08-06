using WebSocketSharp.Server;

namespace SdtdServerKit.WebSockets
{
    internal static class Broadcaster
    {
        private static WebSocketSessionManager _webSocketSessionManager = null!;
        public static void Init(WebSocketSessionManager? webSocketSessionManager)
        {
            if(webSocketSessionManager == null)
                throw new ArgumentNullException(nameof(webSocketSessionManager));
            _webSocketSessionManager = webSocketSessionManager;
        }

        private static void Broadcast(ModEventType modEventType)
        {
            try
            {
                if (_webSocketSessionManager.Count == 0)
                {
                    return;
                }

                string json = JsonConvert.SerializeObject(new WebSocketMessage(modEventType), ModApi.JsonSerializerSettings);
                _webSocketSessionManager.BroadcastAsync(json, null);
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
                if (_webSocketSessionManager.Count == 0)
                {
                    return;
                }

                string json = JsonConvert.SerializeObject(new WebSocketMessage<TData>(modEventType, data), ModApi.JsonSerializerSettings);
                _webSocketSessionManager.BroadcastAsync(json, null);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in ModEventHook.Broadcast");
            }
        }

        static Broadcaster()
        {
            ModEventHub.LogCallback += (entry) => Broadcast(ModEventType.LogCallback, entry);
            ModEventHub.GameAwake += () => Broadcast(ModEventType.GameAwake);
            ModEventHub.GameStartDone += () => Broadcast(ModEventType.GameStartDone);
            ModEventHub.GameUpdate += () => Broadcast(ModEventType.GameUpdate);
            ModEventHub.GameShutdown += () => Broadcast(ModEventType.GameShutdown);
            ModEventHub.ChatMessage += (message) => Broadcast(ModEventType.ChatMessage, message);
            ModEventHub.EntityKilled += (killedEntity) => Broadcast(ModEventType.EntityKilled, killedEntity);
            ModEventHub.EntitySpawned += () => Broadcast(ModEventType.EntitySpawned);
            ModEventHub.PlayerDisconnected += (onlinePlayer) => Broadcast(ModEventType.PlayerDisconnected, onlinePlayer);
            ModEventHub.PlayerLogin += (onlinePlayer) => Broadcast(ModEventType.PlayerLogin, onlinePlayer);
            ModEventHub.PlayerSpawnedInWorld += (spawnedPlayer) => Broadcast(ModEventType.PlayerSpawnedInWorld, spawnedPlayer);
            ModEventHub.PlayerSpawning += (onlinePlayer) => Broadcast(ModEventType.PlayerSpawning, onlinePlayer);
            ModEventHub.SavePlayerData += (onlinePlayer) => Broadcast(ModEventType.SavePlayerData, onlinePlayer);
            ModEventHub.SkyChanged += (skyChanged) => Broadcast(ModEventType.SkyChanged, skyChanged);
        }
    }
}
