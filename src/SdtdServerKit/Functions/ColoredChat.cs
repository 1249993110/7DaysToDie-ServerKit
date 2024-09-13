using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// Colored Chat Function
    /// </summary>
    public class ColoredChat : FunctionBase<ColoredChatSettings>
    {
        private readonly IColoredChatRepository _coloredChatRepository;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="coloredChatRepository"></param>
        public ColoredChat(IColoredChatRepository coloredChatRepository)
        {
            _coloredChatRepository = coloredChatRepository;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnSettingsChanged()
        {
            if (Settings.IsEnabled)
            {
                // Delay register handler until game start done
                if (ModApi.IsGameStartDone == false)
                {
                    ModEventHub.GameStartDone += () => { ModEvents.ChatMessage.RegisterHandler(OnChatMessage); };
                }
                else
                {
                    ModEvents.ChatMessage.RegisterHandler(OnChatMessage);
                }
            }
            else
            {
                ModEvents.ChatMessage.UnregisterHandler(OnChatMessage);
            }
        }

        private bool OnChatMessage(ClientInfo? clientInfo, EChatType eChatType, int senderEntityId, string message, string mainName, List<int> recipientEntityIds)
        {
            if (LivePlayerManager.TryGetByEntityId(senderEntityId, out var managedPlayer))
            {
                var coloredChat = _coloredChatRepository.GetById(managedPlayer!.PlayerId);
                if (coloredChat != null)
                {
                    string playerName = managedPlayer.PlayerName;

                    message = $"[{coloredChat.NameColor}]{playerName}[{GetDefaultColor(eChatType)}]: [{coloredChat.TextColor}]{message}";

                    if (recipientEntityIds != null)
                    {
                        foreach (var entityId in recipientEntityIds)
                        {
                            if(LivePlayerManager.TryGetByEntityId(entityId, out var recipientPlayer))
                            {
                                recipientPlayer!.ClientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>()
                                    .Setup(eChatType, senderEntityId, message, null, EMessageSender.None));
                            }
                        }
                    }
                    else
                    {
                        ConnectionManager.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageChat>()
                            .Setup(eChatType, senderEntityId, message, null, EMessageSender.None), true, -1, -1, -1, null, 192);
                    }

                    return false;
                }
            }

            return true;
        }

        private static string GetDefaultColor(EChatType eChatType)
        {
            switch (eChatType)
            {
                case EChatType.Global:
                    return "FFFFFF";
                case EChatType.Friends:
                    return "00BB00";
                case EChatType.Party:
                    return "FFCC00";
                case EChatType.Whisper:
                    return "D00000";
                default:
                    throw new ArgumentOutOfRangeException(nameof(eChatType), eChatType, null);
            }
        }
    }
}
