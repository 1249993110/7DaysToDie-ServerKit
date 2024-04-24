namespace SdtdServerKit
{
    public static class Utils
    {
        public static IEnumerable<string> SendGlobalMessage(GlobalMessage globalMessage)
        {
            string cmd = string.Format("ty-say {0} {1}", 
                FormatCommandArgs(globalMessage.Message), 
                FormatCommandArgs(globalMessage.SenderName));
            return ExecuteConsoleCommand(cmd);
        }
        public static IEnumerable<string> SendPrivateMessage(PrivateMessage privateMessage)
        {
            string cmd = string.Format("ty-pm {0} {1} {2}",
                FormatCommandArgs(privateMessage.TargetPlayerIdOrName),
                FormatCommandArgs(privateMessage.Message),
                FormatCommandArgs(privateMessage.SenderName));

            return ExecuteConsoleCommand(cmd);
        }
        public static IEnumerable<string> ExecuteConsoleCommand(string command, bool inMainThread = false)
        {
            if (inMainThread)
            {
                IEnumerable<string> executeResult = Enumerable.Empty<string>();
                ModApi.MainThreadSyncContext.Send((state) =>
                {
                    executeResult = SdtdConsole.Instance.ExecuteSync((string)state, ModApi.CmdExecuteDelegate);
                }, command);

                return executeResult;
            }
            else
            {
                return SdtdConsole.Instance.ExecuteSync(command, ModApi.CmdExecuteDelegate);
            }
        }

        public static int DaysRemaining(int daysUntilHorde)
        {
            int bloodmoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            if (daysUntilHorde <= bloodmoonFrequency)
            {
                int daysLeft = bloodmoonFrequency - daysUntilHorde;
                return daysLeft;
            }
            else
            {
                int daysLeft = daysUntilHorde - bloodmoonFrequency;
                return DaysRemaining(daysLeft);
            }
        }

        public static string FormatCommandArgs(string? args)
        {
            if (args == null)
            {
                return string.Empty;
            }

            if (args[0] == '\"' && args[args.Length - 1] == '\"')
            {
                return args;
            }

            if (args.Contains('\"'))
            {
                throw new Exception("Parameters should not contain the character double quotes.");
            }

            if (args.Contains(' '))
            {
                return string.Concat("\"", args, "\"");
            }

            return args;
        }
    }
}