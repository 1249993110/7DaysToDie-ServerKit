namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Command to save all world static xml data to a folder.
    /// </summary>
    public class SaveXmlsToFolder : ConsoleCmdBase
    {
        /// <inheritdoc/>
        public override string getDescription()
        {
            return "Save all world static xml data to a folder.";
        }

        /// <inheritdoc/>
        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-sxtf \"C:/Users/Administrator/Desktop/Temp\"" +
                "1. Save all world static xml data to a specified folder.";
        }

        /// <inheritdoc/>
        public override string[] getCommands()
        {
            return new string[]
            {
                    "ty-SaveXmlsToFolder",
                    "ty-sxtf",
            };
        }

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            if (args.Count < 1)
            {
                Log("Wrong number of arguments, expected 1, found " + args.Count + ".");
                return;
            }

            string path = args[0];
            if (Directory.Exists(path) == false)
            {
                Log("The specified directory: {0} does not exist.", path);
            }
            else
            {
                WorldStaticData.SaveXmlsToFolder(path);
            }
        }
    }
}