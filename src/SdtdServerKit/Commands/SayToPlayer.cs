namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Send a message to a single player.
    /// </summary>
    public class SayToPlayer : ConsoleCmdBase
    {
        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        /// <returns>The description of the command.</returns>
        public override string getDescription()
        {
            return "Sends a message to a single player.";
        }

        /// <summary>
        /// Gets the help text for the command.
        /// </summary>
        /// <returns>The help text for the command.</returns>
        public override string getHelp()
        {
            return "Usage:\n" +
                   "  1. ty-pm <EntityId/PlayerId/PlayerName> <Message>\n" +
                   "1. Sends a PM to the player given by the entity id or player id or player name (as given by e.g. \"lpi\").";
        }

        /// <summary>
        /// Gets the list of commands associated with the command.
        /// </summary>
        /// <returns>The list of commands associated with the command.</returns>
        public override string[] getCommands()
        {
            return new[] { "ty-SayToPlayer", "ty-pm" };
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="args">The list of arguments passed to the command.</param>
        /// <param name="senderInfo">The information of the command sender.</param>
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