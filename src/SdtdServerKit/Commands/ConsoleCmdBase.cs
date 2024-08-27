namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Base class for console commands.
    /// </summary>
    public abstract class ConsoleCmdBase : ConsoleCmdAbstract
    {
        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="line">The message to log.</param>
        protected virtual void Log(string line)
        {
            SdtdConsole.Instance.Output(CustomLogger.Prefix + line);
        }

        /// <summary>
        /// Logs a formatted message to the console.
        /// </summary>
        /// <param name="line">The format string of the message to log.</param>
        /// <param name="args">The arguments to format the message.</param>
        protected virtual void Log(string line, params object[] args)
        {
            SdtdConsole.Instance.Output(CustomLogger.Prefix + string.Format(line, args));
        }
    }
}