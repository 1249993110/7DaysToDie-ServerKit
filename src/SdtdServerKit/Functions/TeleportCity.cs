using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Managers;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 城市传送
    /// </summary>
    public class TeleportCity : FunctionBase<TeleportCitySettings>
    {
        private readonly ICityLocationRepository _cityLocationRepository;
        private readonly IPointsInfoRepository _pointsInfoRepository;
        private readonly ITeleRecordRepository _teleRecordRepository;
        /// <inheritdoc/>
        public TeleportCity(
            ICityLocationRepository cityLocationRepository,
            IPointsInfoRepository pointsInfoRepository,
            ITeleRecordRepository teleRecordRepository)
        {
            _pointsInfoRepository = pointsInfoRepository;
            _teleRecordRepository = teleRecordRepository;
            _cityLocationRepository = cityLocationRepository;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnChatCmd(string message, OnlinePlayer onlinePlayer)
        {
            if (string.Equals(message, Settings.QueryListCmd, StringComparison.OrdinalIgnoreCase))
            {
                string playerId = onlinePlayer.CrossplatformId;
                var cityPositions = await _cityLocationRepository.GetAllAsync();

                if (cityPositions.Any() == false)
                {
                    SendMessageToPlayer(playerId, Settings.NoLocation);
                }
                else
                {
                    int index = 0;
                    foreach (var item in cityPositions)
                    {
                        index++;
                        SendMessageToPlayer(playerId, FormatCmd(Settings.LocationItemTip, onlinePlayer, item, serialNumber: index));
                    }
                }

                return true;
            }
            else if (message.StartsWith(Settings.TeleCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                string cityName = message.Substring(Settings.TeleCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length);
                var cityPosition = await _cityLocationRepository.GetByNameAsync(cityName);
                if (cityPosition == null)
                {
                    return false;
                }
                else
                {
                    string playerId = onlinePlayer.CrossplatformId;

                    var teleRecord = await _teleRecordRepository.GetNewestAsync(playerId, TeleTargetType.City);
                    if (teleRecord != null)
                    {
                        int timeSpan = (int)(DateTime.Now - teleRecord.CreatedAt).TotalSeconds;
                        if (timeSpan < Settings.TeleInterval) // 正在冷却
                        {
                            SendMessageToPlayer(playerId, FormatCmd(Settings.CoolingTip, onlinePlayer, cityPosition, Settings.TeleInterval - timeSpan));

                            return true;
                        }
                    }

                    int pointsCount = await _pointsInfoRepository.GetPointsByIdAsync(playerId);
                    if (pointsCount < cityPosition.PointsRequired) // 积分不足
                    {
                        SendMessageToPlayer(playerId, FormatCmd(Settings.PointsNotEnoughTip, onlinePlayer, cityPosition));
                    }
                    else
                    {
                        await _pointsInfoRepository.ChangePointsAsync(playerId, -cityPosition.PointsRequired);
                        Utils.TeleportPlayer(onlinePlayer.EntityId.ToString(), cityPosition.Position);
                        SendGlobalMessage(FormatCmd(Settings.TeleSuccessTip, onlinePlayer, cityPosition));
                        
                        await _teleRecordRepository.InsertAsync(new T_TeleRecord()
                        {
                            CreatedAt = DateTime.Now,
                            PlayerId = playerId,
                            PlayerName = onlinePlayer.PlayerName,
                            OriginPosition = Utils.GetPlayerPosition(onlinePlayer.EntityId).ToString(),
                            TargetPosition = cityPosition.Position,
                            TargetType = TeleTargetType.City.ToString(),
                            TargetName = cityPosition.CityName
                        });

                        CustomLogger.Info("Player: {0}, entityId: {1}, teleported to: {2}", onlinePlayer.PlayerName, onlinePlayer.EntityId, cityPosition.CityName);
                    }
                }

                return true;
            }

            return false;
        }

        private string FormatCmd(string message, OnlinePlayer player, T_CityLocation position, int cooldownSeconds = 0, int serialNumber = 0)
        {
            return StringTemplate.Render(message, new TeleportCityVariables()
            {
                EntityId = player.EntityId,
                PlatformId = player.PlatformId,
                PlayerName = player.PlayerName,
                PointsRequired = position.PointsRequired,
                CityName = position.CityName,
                TeleInterval = Settings.TeleInterval,
                CooldownSeconds = cooldownSeconds,
                SerialNumber = serialNumber
            });
        }
    }
}