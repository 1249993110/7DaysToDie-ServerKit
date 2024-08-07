﻿namespace SdtdServerKit.Commands
{
    public class GlobalMessage : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Sends a message to all connected clients.";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
               "  1. ty-gm <Message>\n" +
               "1. Sends a message to all connected clients by sender name.";
        }

        public override string[] getCommands()
        {
            return new string[]
            {
                "ty-GlobalMessage",
                "ty-gm",
                "ty-say"
            };
        }

        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            if (args.Count < 1)
            {
                Log("Wrong number of arguments, expected 1, found " + args.Count + ".");
                return;
            }

            string message = args[0];
            GameManager.Instance.ChatMessageServer(ModApi.CmdExecuteDelegate, EChatType.Global, -1, message, null, EMessageSender.Server);
        }
    }
}