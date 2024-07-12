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
  
        private void SendMessage(ClientInfo receiver, ClientInfo? sender, string message, string senderName)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            string senderId;

            if (sender == null || sender.PlatformId == null)
            {
                senderId = Common.NonPlayer;
            }
            else
            {
                senderId = sender.PlatformId.CombinedString;
                senderName = sender.playerName;
            }

            //receiver.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, message, null, EMessageSender.Server));

            CustomLogger.Info("Message \"{0}\" to player {1} sent with sender {2}.", message, receiver.PlatformId.CombinedString, senderId);
            
        }

        private void InternalExecute(int senderEntityId, List<string> args)
        {
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

        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            // From game client.
            if (senderInfo.RemoteClientInfo != null)
            {
                InternalExecute(senderInfo.RemoteClientInfo.entityId, args);
            }
            // From console.
            else
            {
                InternalExecute(-1, args);
            }
        }
    }
}