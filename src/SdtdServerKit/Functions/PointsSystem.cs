﻿using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 积分系统
    /// </summary>
    public class PointsSystem : FunctionBase<PointsSystemSettings>
    {
        private readonly IPointsInfoRepository _pointsInfoRepository;
        /// <inheritdoc/>
        public PointsSystem(IPointsInfoRepository pointsInfoRepository)
        {
            _pointsInfoRepository = pointsInfoRepository;
        }

        /// <inheritdoc/>

        protected override async Task<bool> OnChatCmd(string message, OnlinePlayer player)
        {
            string playerId = player.CrossplatformId;
            // 签到命令
            if (string.Equals(message, Settings.SignInCmd, StringComparison.OrdinalIgnoreCase))
            {
                DateTime now = DateTime.Now;
                var pointsInfo = await _pointsInfoRepository.GetByIdAsync(playerId);

                // 无签到记录
                if (pointsInfo == null)
                {
                    pointsInfo = new T_PointsInfo()
                    {
                        Id = playerId,
                        CreatedAt = now,
                        PlayerName = player.PlayerName,
                        Points = Settings.SignInRewardPoints,
                        LastSignInAt = now,
                    };

                    await _pointsInfoRepository.InsertAsync(pointsInfo);
                }
                else
                {
                    if(pointsInfo.LastSignInAt.HasValue == false || (now - pointsInfo.LastSignInAt.Value).Seconds > Settings.SignInInterval)
                    {
                        pointsInfo.LastSignInAt = now;
                        pointsInfo.Points += Settings.SignInRewardPoints;
                        pointsInfo.PlayerName = player.PlayerName;
                        await _pointsInfoRepository.UpdateAsync(pointsInfo);
                    }
                    else
                    {
                        // 已经签到
                        SendMessageToPlayer(playerId, this.FormatCmd(Settings.SignInFailureTip, player, pointsInfo.Points));
                        return true;
                    }
                }

                SendMessageToPlayer(playerId, this.FormatCmd(Settings.SignInSuccessTip, player, pointsInfo.Points));

                CustomLogger.Info("Player sign in, playerId: {0}, playerName: {1}", playerId, player.PlayerName);
                return true;
            }
            // 查询积分命令
            else if (string.Equals(message, Settings.QueryPointsCmd, StringComparison.OrdinalIgnoreCase))
            {
                var pointsInfo = await _pointsInfoRepository.GetByIdAsync(playerId);

                if (pointsInfo == null)
                {
                    SendMessageToPlayer(playerId, this.FormatCmd(Settings.QueryPointsTip, player, 0));
                }
                else
                {
                    SendMessageToPlayer(playerId, this.FormatCmd(Settings.QueryPointsTip, player, pointsInfo.Points));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private string FormatCmd(string message, IPlayer player, int playerTotalPoints)
        {
            return StringTemplate.Render(message, new PointsSystemVariables()
            {
                EntityId = player.EntityId,
                PlayerTotalPoints = playerTotalPoints,
                PlatformId = player.PlatformId,
                PlayerName = player.PlayerName,
                SignInRewardPoints = Settings.SignInRewardPoints,
            });
        }
    }
}