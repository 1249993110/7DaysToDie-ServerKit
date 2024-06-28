using SdtdServerKit.Hooks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SdtdServerKit.Commands
{
    public class RestartServer : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Restart server, optional parameter -f/force.";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-rs" +
                "  2. ty-rs -f" +
                "1. Restart server by shutdown" +
                "2. Force restart server";
        }

        public override string[] getCommands()
        {
            return new[] { "ty-rs", "ty-RestartServer" };
        }

        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            Log("Server is restarting..., please wait");

            if (args.Count > 0)
            {
                if (string.Equals(args[0], "-f", StringComparison.OrdinalIgnoreCase) 
                    || string.Equals(args[0], "-force", StringComparison.OrdinalIgnoreCase))
                {
                    PrepareRestart(true);
                    return;
                }
            }

            PrepareRestart(false);
        }

        private static volatile bool _isRestarting;

        /// <summary>
        /// 准备重启服务器
        /// </summary>
        /// <param name="force"></param>
        public static void PrepareRestart(bool force = false)
        {
            SdtdConsole.Instance.ExecuteSync("sa", ModApi.CmdExecuteDelegate);

            _isRestarting = true;

            if (force)
            {
                Restart();
            }
            else
            {
                SdtdConsole.Instance.ExecuteSync("shutdown", ModApi.CmdExecuteDelegate);
            }
        }

        private static void Restart()
        {
            string? scriptName = null;
            string? serverPath = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                scriptName = "restart-windows.bat";
                serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startdedicated.bat");
                string path = Path.Combine(ModApi.ModInstance.Path, scriptName);
                Process.Start(path, string.Format("{0} \"{1}\"", Process.GetCurrentProcess().Id, serverPath));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                scriptName = "restart-linux.sh";
                serverPath = AppDomain.CurrentDomain.BaseDirectory;
                Process.Start("chmod", " +x " + Path.Combine(ModApi.ModInstance.Path, scriptName)).WaitForExit();
                string path = Path.Combine(ModApi.ModInstance.Path, scriptName);
                Process.Start(path, string.Format("{0} {1}", Process.GetCurrentProcess().Id, ModApi.AppSettings.ServerSettingsFileName));
            }
        }

        public RestartServer()
        {
            ModEventHook.GameShutdown -= OnGameShutdown;
            ModEventHook.GameShutdown += OnGameShutdown;
        }

        private static void OnGameShutdown()
        {
            if (_isRestarting)
            {
                Restart();
            }
        }
    }
}