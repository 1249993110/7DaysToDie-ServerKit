namespace SdtdServerKit
{
    internal static class CustomLogger
    {
        public const string Prefix = "[LSTY] ";

        public static void Error(string message)
        {
            message = Prefix + message;
            Log.Error(message);
            SdtdConsole.Instance.Output(message);
        }

        public static void Error(string message, params object[] args)
        {
            message = Prefix + message;
            Log.Error(message, args);
            SdtdConsole.Instance.Output(message, args);
        }

        public static void Error(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;
            Log.Error(message);
            SdtdConsole.Instance.Output(message);
        }

        public static void Error(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;
            Log.Error(message, args);
            SdtdConsole.Instance.Output(message, args);
        }

        public static void Info(string message)
        {
            message = Prefix + message;
            Log.Out(message);
            SdtdConsole.Instance.Output(message);
        }

        public static void Info(string message, params object[] args)
        {
            message = Prefix + message;
            Log.Out(message, args);
            SdtdConsole.Instance.Output(message, args);
        }

        public static void Info(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;
            Log.Out(message);
            SdtdConsole.Instance.Output(message);
        }

        public static void Info(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;
            Log.Out(message, args);
            SdtdConsole.Instance.Output(message, args);
        }

        public static void Warn(string message)
        {
            message = Prefix + message;
            Log.Warning(message);
            SdtdConsole.Instance.Output(message);
        }

        public static void Warn(string message, params object[] args)
        {
            message = Prefix + message;
            Log.Warning(message, args);
            SdtdConsole.Instance.Output(message, args);
        }

        public static void Warn(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;
            Log.Warning(message);
            SdtdConsole.Instance.Output(message);
        }

        public static void Warn(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;
            Log.Warning(message, args);
            SdtdConsole.Instance.Output(message, args);
        }
    }
}