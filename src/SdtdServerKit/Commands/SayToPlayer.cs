namespace SdtdServerKit.Commands
{
    public class SayToPlayer : ConsoleCmdBase
    {
        public override string getDescription()
        {
            return "Send a message to a single player.";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                   "  1. ty-pm <EntityId/PlayerId/PlayerName> <Message>\n" +
                   "1. Send a PM to the player given by the entity id or player id or player name (as given by e.g. \"lpi\").";
        }

        public override string[] getCommands()
        {
            return new[] { "ty-SayToPlayer", "ty-pm" };
        }

        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            int senderEntityId = -1; // From console.
            // From game client.
            if (senderInfo.RemoteClientInfo != null)
            {
                senderEntityId = senderInfo.RemoteClientInfo.entityId;
            }

            if (args.Count < 2)
            {
                Log(getHelp());
                return;
            }

            string message = args[1];

            ClientInfo receiver = ConsoleHelper.ParseParamIdOrName(args[0]);
            if (receiver == null)
            {
                Log("Unable to locate player '{0}' online", args[0]);
            }
            else
            {
                GameManager.Instance.ChatMessageServer(
                    ModApi.CmdExecuteDelegate,
                    EChatType.Whisper,
                    senderEntityId,
                    message,
                    new List<int>() { receiver.entityId },
                    EMessageSender.Server);
            }
        }
    }
}