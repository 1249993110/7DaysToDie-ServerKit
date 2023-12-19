using HarmonyLib;
using IceCoffee.Common.Timers;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Serialization;
using Platform.Local;
using SdtdServerKit.Hooks;
using SdtdServerKit.Triggers;
using SdtdServerKit.WebApi;
using System.Text;
using WebSocketSharp.Server;
using SdtdServerKit.Commands;
using MapRendering;

namespace SdtdServerKit
{
    /// <summary>
    /// ModApi
    /// </summary>
    public class ModApi : IModApi
    {
        /// <summary>
        /// ModInstance
        /// </summary>
        internal static Mod ModInstance { get; private set; } = null!;

        /// <summary>
        /// AppSettings
        /// </summary>
        internal static AppSettings AppSettings { get; private set; } = null!;

        /// <summary>
        /// Main thread(the ui thread) synchronization context
        /// </summary>
        internal static SynchronizationContext MainThreadSyncContext { get; private set; } = null!;

        internal static ClientInfo CmdExecuteDelegate { get; private set; } = null!;

        internal static bool IsGameStartDone { get; private set; }

        internal static JsonSerializerSettings JsonSerializerSettings { get; private set; } = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        /// <summary>
        /// InitMod
        /// </summary>
        /// <param name="modInstance"></param>
        public void InitMod(Mod modInstance)
        {
            try
            {
                ModInstance = modInstance;
                MainThreadSyncContext = SynchronizationContext.Current;
                CmdExecuteDelegate = new ClientInfo()
                {
                    PlatformId = new UserIdentifierLocal(modInstance.Name),
                };

                LoadAppSettings();

                StartupOwinHost();

                PatchByHarmony();

                RegisterModEventHandlers();
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Initialize mod: " + modInstance.Name + " failed.");
            }
        }

        private static void LoadAppSettings()
        {
            try
            {
                string path = Path.Combine(ModInstance.Path, "appsettings.json");
                string json = File.ReadAllText(path, Encoding.UTF8);

                var appSettings = JsonConvert.DeserializeObject<AppSettings>(json);
                if (appSettings == null)
                {
                    throw new Exception("The app settings can not be null.");
                }
                else
                {
                    AppSettings = appSettings;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Load appsettings failed.", ex);
            }
        }

        private static WebSocketServer? _webSocketServer;
        internal static WebSocketSessionManager WebSocketSessionManager { get; private set; } = null!;
        private static void StartupOwinHost()
        {
            try
            {
                string webUrl = AppSettings.WebUrl;
                WebApp.Start<Startup>(webUrl);
                CustomLogger.Info("SdtdServerKit Web App Server running on " + webUrl);

                int webSocketPort = AppSettings.WebSocketPort;
                _webSocketServer = new WebSocketServer(webSocketPort);
                _webSocketServer.AddWebSocketService<WebSockets.Telnet>("/ws");                
                _webSocketServer.Start();
                WebSocketSessionManager = _webSocketServer.WebSocketServices["/ws"].Sessions;

                if (_webSocketServer.IsListening)
                {
                    CustomLogger.Info("SdtdServerKit Web Socket Server listening on port " + webSocketPort);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Startup Owin Host Server failed.", ex);
            }
        }

        private static Harmony _harmony = null!;
        private static void PatchByHarmony()
        {
            try
            {
                _harmony = new Harmony(ModInstance.Name);
                _harmony.PatchAll(typeof(ModApi).Assembly);

                CustomLogger.Info("Successfully patch all by harmony.");
            }
            catch (Exception ex)
            {
                throw new Exception("Patch by harmony failed.", ex);
            }
        }


        private static MapTileCache _mapTileCache = null!;
        internal static MapTileCache GetMapTileCache() => _mapTileCache;
        private static void RegisterModEventHandlers()
        {
            try
            {
                Log.LogCallbacks += ModEventHook.OnLogCallback;
                ModEvents.GameAwake.RegisterHandler(ModEventHook.OnGameAwake);
                ModEvents.GameStartDone.RegisterHandler(() =>
                {
                    WorldStaticDataHook.ReplaceXmlsToImplRemovePlayerItems();
                    _mapTileCache = (MapTileCache)MapRenderer.GetTileCache();
                    ModEventHook.OnGameStartDone();
                    IsGameStartDone = true;
                });
                ModEvents.GameShutdown.RegisterHandler(() =>
                {
                    ModEventHook.OnGameShutdown();
                    RestartServer.OnGameShutdown();
                });
                ModEvents.PlayerLogin.RegisterHandler(ModEventHook.OnPlayerLogin);
                ModEvents.PlayerSpawnedInWorld.RegisterHandler(ModEventHook.OnPlayerSpawnedInWorld);
                ModEvents.EntityKilled.RegisterHandler(ModEventHook.OnEntityKilled);
                ModEvents.PlayerDisconnected.RegisterHandler(ModEventHook.OnPlayerDisconnected);
                ModEvents.SavePlayerData.RegisterHandler(ModEventHook.OnSavePlayerData);
                ModEvents.ChatMessage.RegisterHandler(ModEventHook.OnChatMessage);
                ModEvents.PlayerSpawning.RegisterHandler(ModEventHook.OnPlayerSpawning);

                GlobalTimer.RegisterSubTimer(new SubTimer(SkyChangeTrigger.Callback, 1) { IsEnabled = true });

                CustomLogger.Info("Successfully registered mod event handlers.");
            }
            catch (Exception ex)
            {
                throw new Exception("Register mod event handlers failed.", ex);
            }
        }
    }
}
