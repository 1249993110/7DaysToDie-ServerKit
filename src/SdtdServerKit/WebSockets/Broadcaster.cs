using SdtdServerKit.Hooks;
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
            ModEventHook.LogCallback += (entry) => Broadcast(ModEventType.LogCallback, entry);
            ModEventHook.GameAwake += () => Broadcast(ModEventType.GameAwake);
            ModEventHook.GameStartDone += () => Broadcast(ModEventType.GameStartDone);
            ModEventHook.GameUpdate += () => Broadcast(ModEventType.GameUpdate);
            ModEventHook.GameShutdown += () => Broadcast(ModEventType.GameShutdown);
            ModEventHook.ChatMessage += (message) => Broadcast(ModEventType.ChatMessage, message);
            ModEventHook.EntityKilled += (killedEntity) => Broadcast(ModEventType.EntityKilled, killedEntity);
            ModEventHook.EntitySpawned += () => Broadcast(ModEventType.EntitySpawned);
            ModEventHook.PlayerDisconnected += (onlinePlayer) => Broadcast(ModEventType.PlayerDisconnected, onlinePlayer);
            ModEventHook.PlayerLogin += (onlinePlayer) => Broadcast(ModEventType.PlayerLogin, onlinePlayer);
            ModEventHook.PlayerSpawnedInWorld += (spawnedPlayer) => Broadcast(ModEventType.PlayerSpawnedInWorld, spawnedPlayer);
            ModEventHook.PlayerSpawning += (onlinePlayer) => Broadcast(ModEventType.PlayerSpawning, onlinePlayer);
            ModEventHook.SavePlayerData += (onlinePlayer) => Broadcast(ModEventType.SavePlayerData, onlinePlayer);
            ModEventHook.SkyChanged += (skyChanged) => Broadcast(ModEventType.SkyChanged, skyChanged);
        }
    }
}
