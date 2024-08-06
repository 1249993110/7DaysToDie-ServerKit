using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;
using SdtdServerKit.Variables;
using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit.Functions
{
    public class TeleportHome : FunctionBase<TeleportHomeSettings>
    {
        private readonly IHomeLocationRepository _homeLocationRepository;
        private readonly IPointsInfoRepository _pointsRepository;
        private readonly ITeleRecordRepository _teleRecordRepository;
        /// <inheritdoc/>
        public TeleportHome(
            IPointsInfoRepository pointsInfoRepository,
            ITeleRecordRepository teleRecordRepository,
            IHomeLocationRepository homeLocationRepository)
        {
            _pointsRepository = pointsInfoRepository;
            _teleRecordRepository = teleRecordRepository;
            _homeLocationRepository = homeLocationRepository;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnChatCmd(string message, OnlinePlayer onlinePlayer)
        {
            if (string.Equals(message, Settings.QueryListCmd, StringComparison.OrdinalIgnoreCase))
            {
                int entityId = onlinePlayer.EntityId;
                string playerId = onlinePlayer.PlayerId;
                var positions = await _homeLocationRepository.GetByPlayerIdAsync(playerId);

                if (positions.Any() == false)
                {
                    SendMessageToPlayer(playerId, Settings.NoHomeTip);
                }
                else
                {
                    foreach (var item in positions)
                    {
                        SendMessageToPlayer(playerId, FormatCmd(Settings.LocationItemTip, onlinePlayer, item));
                    }
                }

                return true;
            }
            else if (message.StartsWith(Settings.SetHomeCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                string homeName = message.Substring(Settings.SetHomeCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length);
                int entityId = onlinePlayer.EntityId;
                string playerId = onlinePlayer.PlayerId;

                int playerPoints = await _pointsRepository.GetPointsByIdAsync(playerId);
                if (playerPoints < Settings.PointsRequiredForSet)
                {
                    SendMessageToPlayer(playerId, FormatCmd(Settings.SetPointsNotEnoughTip, onlinePlayer));
                }
                else
                {
                    var entity = await _homeLocationRepository.GetByPlayerIdAndHomeNameAsync(playerId, homeName);
                    string position = Utils.GetPlayerPosition(onlinePlayer.EntityId).ToString();
                    // new home postion
                    if (entity == null)
                    {
                        var positionCount = await _homeLocationRepository.GetRecordCountByPlayerIdIdAsync(playerId);
                        if (positionCount >= Settings.SetCountLimit)
                        {
                            SendMessageToPlayer(playerId, FormatCmd(Settings.OverLimitTip, onlinePlayer));
                            return true;
                        }
                        else
                        {
                            entity = new T_HomeLocation()
                            {
                                HomeName = homeName,
                                PlayerId = playerId,
                                CreatedAt = DateTime.Now,
                                PlayerName = onlinePlayer.PlayerName,
                                Position = position
                            };
                            await _homeLocationRepository.InsertAsync(entity);
                            SendMessageToPlayer(playerId, FormatCmd(Settings.SetSuccessTip, onlinePlayer, entity));
                        }
                    }
                    else
                    {
                        entity.HomeName = homeName;
                        entity.PlayerName = onlinePlayer.PlayerName;
                        entity.Position = position;
                        await _homeLocationRepository.UpdateAsync(entity);
                        SendMessageToPlayer(playerId, FormatCmd(Settings.OverwriteSuccessTip, onlinePlayer, entity));
                    }

                    await _pointsRepository.ChangePointsAsync(playerId, -Settings.PointsRequiredForSet);
                    CustomLogger.Info("Player: {0}, entityId: {1}, set home: {2}, position: {3}", onlinePlayer.PlayerName, entityId, homeName, position);
                }

                return true;
            }
            else if (message.StartsWith(Settings.DeleteHomeCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                string homeName = message.Substring(Settings.DeleteHomeCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length);
                int entityId = onlinePlayer.EntityId;
                string playerId = onlinePlayer.PlayerId;
                int count = await _homeLocationRepository.DeleteByPlayerIdAndHomeNameAsync(playerId, homeName);
                if (count != 1)
                {
                    SendMessageToPlayer(playerId, Settings.HomeNotFoundTip);
                }
                else
                {
                    SendMessageToPlayer(playerId, Settings.DeleteSuccessTip);
                }

                return true;
            }
            else if (message.StartsWith(Settings.TeleHomeCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                string homeName = message.Substring(Settings.TeleHomeCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length);
                int entityId = onlinePlayer.EntityId;
                string playerId = onlinePlayer.PlayerId;

                var entity = await _homeLocationRepository.GetByPlayerIdAndHomeNameAsync(playerId, homeName);
                if (entity == null)
                {
                    SendMessageToPlayer(playerId, Settings.HomeNotFoundTip);
                }
                else
                {
                    var teleRecord = await _teleRecordRepository.GetNewestAsync(playerId, TeleTargetType.Home);
                    if (teleRecord != null)
                    {
                        int timeSpan = (int)(DateTime.Now - teleRecord.CreatedAt).TotalSeconds;
                        if (timeSpan < Settings.TeleInterval)// Cooling
                        {
                            SendMessageToPlayer(playerId, FormatCmd(Settings.CoolingTip, onlinePlayer, entity, Settings.TeleInterval - timeSpan));
                            return true;
                        }
                    }

                    int points = await _pointsRepository.GetPointsByIdAsync(playerId);
                    if (points < Settings.PointsRequiredForTele)// Points not enough
                    {
                        SendMessageToPlayer(playerId, FormatCmd(Settings.TelePointsNotEnoughTip, onlinePlayer, entity));
                    }
                    else
                    {
                        if (ConfigManager.GlobalSettings.TeleZombieCheck &&
                            GameManager.Instance.World.Players.dict.TryGetValue(onlinePlayer.EntityId, out EntityPlayer player))
                        {
                            if (Utils.ZombieCheck(player))
                            {
                                SendMessageToPlayer(playerId, ConfigManager.GlobalSettings.TeleDisableTip);
                                return true;
                            }
                        }

                        await _pointsRepository.ChangePointsAsync(playerId, -Settings.PointsRequiredForTele);
                        Utils.TeleportPlayer(onlinePlayer.EntityId.ToString(), entity.Position);

                        SendGlobalMessage(FormatCmd(Settings.TeleSuccessTip, onlinePlayer, entity));
                        await _teleRecordRepository.InsertAsync(new T_TeleRecord()
                        {
                            CreatedAt = DateTime.Now,
                            PlayerId = playerId,
                            PlayerName = onlinePlayer.PlayerName,
                            OriginPosition = Utils.GetPlayerPosition(onlinePlayer.EntityId).ToString(),
                            TargetPosition = entity.Position,
                            TargetType = TeleTargetType.Home.ToString(),
                            TargetName = entity.HomeName
                        });

                        CustomLogger.Info("Player: {0}, entityId: {1}, teleported to: {2}", onlinePlayer.PlayerName, entityId, entity.HomeName);
                    }
                }

                return true;
            }

            return false;
        }

        private string FormatCmd(string message, OnlinePlayer player, T_HomeLocation position, int cooldownSeconds = 0)
        {
            return StringTemplate.Render(message, new TeleportHomeVariables()
            {
                EntityId = player.EntityId,
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                CooldownSeconds = cooldownSeconds,
                HomeName = position.HomeName,
                TeleInterval = Settings.TeleInterval,
            });
        }
    }
}