using HarmonyLib;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Serialization;
using Platform.Local;
using SdtdServerKit.Hooks;
using SdtdServerKit.Triggers;
using SdtdServerKit.WebApi;
using System.Text;
using WebSocketSharp.Server;
using MapRendering;
using Dapper;
using IceCoffee.SimpleCRUD.SqliteTypeHandlers;
using IceCoffee.SimpleCRUD;
using SdtdServerKit.Data;
using System.Reflection;
using Autofac.Integration.WebApi;
using Microsoft.Data.Sqlite;
using SdtdServerKit.Managers;
using SdtdServerKit.WebSockets;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

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

                LoadPlugins();

                PatchByHarmony();

                InitDependencyResolver();

                StartupOwinHost();

                StartupWebSocket();

                RegisterModEventHandlers();

            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Initialize mod: " + modInstance.Name + " failed.");
            }
        }

        /// <summary>
        /// LoadedPlugins
        /// </summary>
        public static IReadOnlyList<Assembly> LoadedPlugins => _loadedPlugins;
        private static List<Assembly> _loadedPlugins = new List<Assembly>() { Assembly.GetExecutingAssembly() };
        private static void LoadPlugins()
        {
            string[] assemblyFiles = Directory.GetFiles(Path.Combine(ModInstance.Path, "Plugins"), "*.dll");
            foreach (var assemblyFile in assemblyFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(assemblyFile);
                    _loadedPlugins.Add(assembly);

                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        if (typeof(IModApi).IsAssignableFrom(type))
                        {
                            string assemblyName = Path.GetFileName(assembly.Location);
                            CustomLogger.Info("Found ModAPI in " + assemblyName + ", creating instance.");
                            var modApi = (IModApi)Activator.CreateInstance(type);
                            try
                            {
                                modApi.InitMod(ModInstance);
                                CustomLogger.Info("Initialized ModAPI instance on mod '" + ModInstance.Name + "' from DLL '" + assemblyName + "'");
                            }
                            catch (Exception ex)
                            {
                                CustomLogger.Error(ex, "Failed initializing ModAPI instance on mod '" + ModInstance.Name + "' from DLL '" + assemblyName + "'");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.Error(ex, "Load plugin: " + assemblyFile + " failed.");
                }
            }
        }

        /// <summary>
        /// 获取默认配置文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string GetDefaultConfigPath(string fileName)
        {
            return Path.Combine(ModInstance.Path, "Config", fileName);
        }

        private static void LoadAppSettings()
        {
            try
            {
                const string LSTY_Data = "LSTY_Data";
                string baseSettingsPath = Path.Combine(AppContext.BaseDirectory, LSTY_Data);
                Directory.CreateDirectory(baseSettingsPath);

                string defaultAppConfigPath = GetDefaultConfigPath("appsettings.json");
                string productionAppConfigPath = Path.Combine(baseSettingsPath, "appsettings.json");

                if(File.Exists(productionAppConfigPath) == false)
                {
                    File.Copy(defaultAppConfigPath, productionAppConfigPath);
                }

                var builder = new ConfigurationBuilder()
                    //.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile(defaultAppConfigPath, optional: false, reloadOnChange: false)
                    .AddJsonFile(productionAppConfigPath, optional: true, reloadOnChange: false);

                var configuration = builder.Build();
                var appSettings = configuration.Get<AppSettings>();
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
        private static void StartupOwinHost()
        {
            try
            {
                string webUrl = AppSettings.WebUrl;
                WebApp.Start<Startup>(webUrl);
                CustomLogger.Info("SdtdServerKit Web App Server running on " + webUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Startup Owin Host Server failed.", ex);
            }
        }

        private static void StartupWebSocket()
        {
            try
            {
                int webSocketPort = AppSettings.WebSocketPort;
                _webSocketServer = new WebSocketServer(webSocketPort);
                _webSocketServer.AddWebSocketService<WebSockets.Telnet>("/ws");
                _webSocketServer.Start();
                if (_webSocketServer.IsListening)
                {
                    var webSocketSessionManager = _webSocketServer.WebSocketServices["/ws"].Sessions;
                    Broadcaster.Init(webSocketSessionManager);
                    CustomLogger.Info("SdtdServerKit Web Socket Server listening on port " + webSocketPort);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Startup Web Socket failed.", ex);
            }
        }

        private static Harmony _harmony = null!;
        public static Harmony Harmony => _harmony;
        private static void PatchByHarmony()
        {
            try
            {
                _harmony = new Harmony(ModInstance.Name);
                _harmony.PatchAll(typeof(ModApi).Assembly);

                CustomLogger.Info("Patch all by harmony success.");
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
                ModEvents.GameShutdown.RegisterHandler(ModEventHook.OnGameShutdown);
                ModEvents.PlayerLogin.RegisterHandler(ModEventHook.OnPlayerLogin);
                ModEvents.PlayerSpawnedInWorld.RegisterHandler(ModEventHook.OnPlayerSpawnedInWorld);
                ModEvents.EntityKilled.RegisterHandler(ModEventHook.OnEntityKilled);
                ModEvents.PlayerDisconnected.RegisterHandler(ModEventHook.OnPlayerDisconnected);
                ModEvents.SavePlayerData.RegisterHandler(ModEventHook.OnSavePlayerData);
                ModEvents.ChatMessage.RegisterHandler(ModEventHook.OnChatMessage);
                ModEvents.PlayerSpawning.RegisterHandler(ModEventHook.OnPlayerSpawning);

                GlobalTimer.RegisterSubTimer(new SubTimer(SkyChangeTrigger.Callback, 1) { IsEnabled = true });

                CustomLogger.Info("Registered mod event handlers success.");
            }
            catch (Exception ex)
            {
                throw new Exception("Register mod event handlers failed.", ex);
            }
        }

        /// <summary>
        /// DependencyResolver
        /// </summary>
        public static IContainer ServiceContainer { get; private set; } = null!;
        
        /// <summary>
        /// 只注册控制器、Function、数据库仓储相关类型
        /// </summary>
        private void InitDependencyResolver()
        {
            var builder = new ContainerBuilder();

            #region 注册数据库仓储服务
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string destPath = Path.Combine(AppContext.BaseDirectory, "7DaysToDieServer_Data/MonoBleedingEdge/x86_64", "libdl.so");
                if (File.Exists(destPath) == false)
                {
                    string srcPath = Path.Combine(ModInstance.Path, "libe_sqlite3.so");
                    File.Copy(srcPath, destPath, true);
                }
            }

            SqlMapper.AddTypeHandler(new GuidHandler());

            string databasePath = Path.Combine(AppContext.BaseDirectory, AppSettings.DatabasePath);

            // 复制旧数据库文件
            if (File.Exists(databasePath) == false)
            {
                string oldDatabasePath = Path.Combine(ModInstance.Path, "Data/database.db");
                if (File.Exists(oldDatabasePath))
                {
                    File.Copy(oldDatabasePath, databasePath);
                    CustomLogger.Info("Copy old database file success.");
                }
            }

            string connectionString = $"Data Source={databasePath};Cache=Shared";
            DbConnectionFactory.Default.ConfigureOptions(new DbConnectionOptions()
            {
                ConnectionString = connectionString,
                DbType = DbType.SQLite,
            });

            foreach (var item in _loadedPlugins)
            {
                builder.AddRepositories(item);
            }

            #endregion

            // Register Functions.
            foreach (var type in FunctionManager.GetFunctionTypes())
            {
                builder.RegisterType(type).SingleInstance();
            }

            // Register your Web API controllers.
            builder.RegisterApiControllers(_loadedPlugins.ToArray());

            // Run other optional steps, like registering filters,
            // per-controller-type services, etc., then set the dependency resolver
            // to be Autofac.
            var container = builder.Build();

            ServiceContainer = container;

            InitDatabase();
            FunctionManager.LoadFunctions();
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        private void InitDatabase()
        {
            try
            {
                var dbConnection = DbConnectionFactory.Default.CreateConnection(DbAliases.Default);

                string dataSource = ((SqliteConnection)dbConnection).DataSource;
                var fileInfo = new FileInfo(dataSource);
                if (fileInfo.Exists == false)
                {
                    if (fileInfo.DirectoryName != null)
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }
                    File.Create(dataSource).Close();
                }

                string path = Path.Combine(ModInstance.Path, "Config/sql");
                var di = new DirectoryInfo(path);
                var files = di.GetFiles("*.sql");

                foreach (var file in files)
                {
                    string sql = File.ReadAllText(file.FullName, Encoding.UTF8);
                    dbConnection.Execute(sql);
                }

                CustomLogger.Info("Initialize database success.");
            }
            catch (Exception ex)
            {
                throw new Exception("Initialize database error.", ex);
            }
        }
    }
}
