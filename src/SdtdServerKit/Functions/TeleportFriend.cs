using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 好友传送
    /// </summary>
    public class TeleportFriend : FunctionBase<TeleportFriendSettings>
    {
        private readonly IPointsInfoRepository _pointsRepository;
        private readonly ITeleRecordRepository _teleRecordRepository;
        /// <inheritdoc/>
        public TeleportFriend(
            IPointsInfoRepository pointsRepository, 
            ITeleRecordRepository teleRecordRepository)
        {
            _pointsRepository = pointsRepository;
            _teleRecordRepository = teleRecordRepository;
        }

        /// <inheritdoc/>
        protected override async Task<bool> OnChatCmd(string message, ManagedPlayer managedPlayer)
        {
            if (message.StartsWith(Settings.TeleCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                string targetName = message.Substring(Settings.TeleCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length);
                string playerId = managedPlayer.PlayerId;

                var targetPlayer = ConnectionManager.Instance.Clients.GetForPlayerName(targetName);
                if (targetPlayer == null)
                {
                    SendMessageToPlayer(playerId, FormatCmd(Settings.TargetNotFoundTip, managedPlayer, targetName));
                    return true;
                }

                if (Utils.IsFriend(managedPlayer.EntityId, targetPlayer.entityId) == false)
                {
                    SendMessageToPlayer(playerId, FormatCmd(Settings.TargetNotFriendTip, managedPlayer, targetName));
                    return true;
                }

                var teleRecord = await _teleRecordRepository.GetNewestAsync(playerId, TeleTargetType.Friend);
                if (teleRecord != null)
                {
                    int timeSpan = (int)(DateTime.Now - teleRecord.CreatedAt).TotalSeconds;
                    if (timeSpan < Settings.TeleInterval)// Cooling
                    {
                        SendMessageToPlayer(playerId, FormatCmd(Settings.CoolingTip, managedPlayer, targetName, Settings.TeleInterval - timeSpan));
                        return true;
                    }
                }

                int pointsCount = await _pointsRepository.GetPointsByIdAsync(playerId);
                if (pointsCount < Settings.PointsRequired)// Points not enough
                {
                    SendMessageToPlayer(playerId, FormatCmd(Settings.PointsNotEnoughTip, managedPlayer, targetName));
                    return true;
                }

                if (ConfigManager.GlobalSettings.TeleZombieCheck &&
                    GameManager.Instance.World.Players.dict.TryGetValue(managedPlayer.EntityId, out EntityPlayer player))
                {
                    if (Utils.ZombieCheck(player))
                    {
                        SendMessageToPlayer(playerId, ConfigManager.GlobalSettings.TeleDisableTip);
                        return true;
                    }
                }

                await _pointsRepository.ChangePointsAsync(playerId, -Settings.PointsRequired);

                Utils.TeleportPlayer(managedPlayer.EntityId.ToString(), targetPlayer.entityId.ToString());
                SendGlobalMessage(FormatCmd(Settings.TeleSuccessTip, managedPlayer, targetName));

                await _teleRecordRepository.InsertAsync(new T_TeleRecord()
                {
                    CreatedAt = DateTime.Now,
                    PlayerId = playerId,
                    PlayerName = managedPlayer.PlayerName,
                    TargetName = targetName,
                    TargetType = TeleTargetType.Friend.ToString(),
                    OriginPosition = Utils.GetPlayerPosition(managedPlayer.EntityId).ToString(),
                    TargetPosition = Utils.GetPlayerPosition(targetPlayer.entityId).ToString(),
                });

                CustomLogger.Info("Player: {0}, entityId: {1}, teleported to: {2}", managedPlayer.PlayerName, managedPlayer.EntityId, targetName);
                return true;
            }

            return false;
        }

        private string FormatCmd(string message, ManagedPlayer player, string targetName, int coolingTime = 0)
        {
            return StringTemplate.Render(message, new TeleportFriendVariables()
            {
                EntityId = player.EntityId,
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                PointsRequired = Settings.PointsRequired,
                TargetName = targetName,
                TeleInterval = Settings.TeleInterval,
                CooldownSeconds = coolingTime
            });
        }
    }
}