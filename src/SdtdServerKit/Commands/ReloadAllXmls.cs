namespace SdtdServerKit.Commands
{
    public class ReloadAllXmls : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Reload all xmls sync.";
        }

        public override string getHelp()
        {
            return "";
        }

        public override string[] getCommands()
        {
            return new string[]
            {
                "ty-ReloadAllXmls",
            };
        }

        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            WorldStaticData.ReloadAllXmlsSync();
            Log("Reloaded all xmls.");
        }
    }
}