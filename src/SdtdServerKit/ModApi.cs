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
using Autofac.Core;
using Autofac;
using Dapper;
using IceCoffee.SimpleCRUD.SqliteTypeHandlers;
using IceCoffee.SimpleCRUD;
using SdtdServerKit.Data;
using System.Reflection;
using Autofac.Integration.WebApi;
using SdtdServerKit.Data.IRepositories;
using Microsoft.Data.Sqlite;
using SdtdServerKit.Managers;
using SdtdServerKit.WebSockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

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

                InitDependencyResolver();

                LoadFileBrowser();

                StartupOwinHost();

                StartupWebSocket();

                PatchByHarmony();

                RegisterModEventHandlers();

            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Initialize mod: " + modInstance.Name + " failed.");
            }
        }

        #region Load FileBrowser
        /// <summary>
        /// 加载文件浏览器
        /// </summary>
        private void LoadFileBrowser()
        {
            try
            {
                string binPath = Path.Combine(ModInstance.Path, "3rdparty-binaries", "filebrowser", "filebrowser");
                LoadFileBrowser(binPath, AppSettings.FileBrowserPort, AppDomain.CurrentDomain.BaseDirectory, AppSettings.UserName, AppSettings.Password);
                CustomLogger.Info("Load file browser success.");
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Load file browser failed.");
            }
        }
        private static Process SilentStartProcess(string fileName, string argument)
        {
            string directory = Path.GetDirectoryName(fileName);

            var process = new Process();
            var startInfo = process.StartInfo;
            startInfo.FileName = fileName;
            startInfo.WorkingDirectory = directory;
            startInfo.Arguments = argument;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden; // 与CreateNoWindow联合使用可以隐藏进程运行的窗体
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            process.Start();
            return process;
        }

        private static void LoadFileBrowser(string binPath, int port, string root, string username, string password)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                binPath += ".exe";
            }
            else
            {
                Process.Start("chmod", " +x \"" + binPath + "\"").WaitForExit();
            }

            if (File.Exists(binPath) == false)
            {
                throw new NotImplementedException("FileBrowser not found.");
            }

            string[] arguments = new string[]
            {
                "config init",
                "config set --address 0.0.0.0",
                $"config set --port {port}",
                "config set --locale zh-cn",
                $"config set --root \"{root}\"",
                $"users add {username} {password} --perm.admin --lockPassword",
                $"users update {username} --password {password} --perm.admin --lockPassword"
            };

            Process process;
            foreach (var argument in arguments)
            {
                process = SilentStartProcess(binPath, argument);
                if (process.HasExited == false)
                {
                    process.WaitForExit(3000);
                }
            }

            process = SilentStartProcess(binPath, string.Empty);
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                if (process.HasExited == false)
                {
                    try
                    {
                        process.WaitForExit(3000); // 等待进程结束
                        if (process.HasExited == false)
                        {
                            process.Kill();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            };

            if (isWindows)
            {
                ChildProcessTracker.AddProcess(process);
            }
        }
        #endregion

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

            string databasePath = Path.Combine(ModInstance.Path, AppSettings.DatabasePath);
            string connectionString = $"Data Source={databasePath};Cache=Shared";
            DbConnectionFactory.Default.ConfigureOptions(new DbConnectionOptions()
            {
                ConnectionString = connectionString,
                DbType = DbType.SQLite,
            });

            var assembly = Assembly.GetExecutingAssembly();
            builder.AddRepositories(assembly);

            #endregion

            // Register Functions.
            foreach (var type in FunctionManager.GetFunctionTypes())
            {
                builder.RegisterType(type).SingleInstance();
            }

            // Register your Web API controllers.
            builder.RegisterApiControllers(assembly);

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

                string path = Path.Combine(ModInstance.Path, "Data/sql");
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
