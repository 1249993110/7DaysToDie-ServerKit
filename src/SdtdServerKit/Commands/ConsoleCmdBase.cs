namespace SdtdServerKit.Commands
{
    public abstract class ConsoleCmdBase : ConsoleCmdAbstract
    {
        protected virtual void Log(string line)
        {
            SdtdConsole.Instance.Output(CustomLogger.Prefix + line);
        }

        protected virtual void Log(string line, params object[] args)
        {
            SdtdConsole.Instance.Output(CustomLogger.Prefix + string.Format(line, args));
        }
    }
}