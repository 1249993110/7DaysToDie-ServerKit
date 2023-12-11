namespace SdtdServerKit.Commands
{
    public class SayToPlayer : ConsoleCmdBase
    {
        protected override string getDescription()
        {
            return "Send a message to a single player.";
        }

        protected override string getHelp()
        {
            return "Usage:\n" +
                   "  1. ty-pm <EntityId/PlayerId/PlayerName> <Message>\n" +
                   "1. Send a PM to the player given by the entity id or player id or player name (as given by e.g. \"lpi\").";
        }

        protected override string[] getCommands()
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

            receiver.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, message, senderName, false, null));

            CustomLogger.Info("Message \"{0}\" to player {1} sent with sender {2}.", message, receiver.PlatformId.CombinedString, senderId);
        }

        private void InternalExecute(ClientInfo? sender, List<string> args)
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
                string senderName = (args.Count < 3 || string.IsNullOrEmpty(args[2])) ? Common.DefaultServerName : args[2];
                SendMessage(receiver, sender, message, senderName);
            }
        }

        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            // From game client.
            if (senderInfo.RemoteClientInfo != null)
            {
                InternalExecute(senderInfo.RemoteClientInfo, args);
            }
            // From console.
            else
            {
                InternalExecute(null, args);
            }
        }
    }
}