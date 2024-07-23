﻿using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.Managers;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// VIP礼包
    /// </summary>
    public class VipGift : FunctionBase<VipGiftSettings>
    {
        private readonly IVipGiftRepository _vipGiftRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly ICommandListRepository _commandListRepository;

        /// <inheritdoc/>
        public VipGift(IVipGiftRepository vipGiftRepository, IItemListRepository itemListRepository, ICommandListRepository commandListRepository)
        {
            _vipGiftRepository = vipGiftRepository;
            _itemListRepository = itemListRepository;
            _commandListRepository = commandListRepository;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnChatCmd(string message, OnlinePlayer onlinePlayer)
        {
            if (string.Equals(message, Settings.ClaimCmd, StringComparison.OrdinalIgnoreCase))
            {
                string playerId = onlinePlayer.CrossplatformId;
                var vipGift = await _vipGiftRepository.GetByIdAsync(playerId);
                if (vipGift == null)
                {
                    SendMessageToPlayer(playerId, Settings.NonVipTip);
                }
                else
                {
                    if (vipGift.ClaimState)
                    {
                        SendMessageToPlayer(playerId, Settings.HasClaimedTip);
                        return true;
                    }

                    var itemList = await _itemListRepository.GetListByVipGiftIdAsync(vipGift.Id);
                    foreach (var item in itemList)
                    {
                        Utils.GiveItem(playerId, item.ItemName, item.Count, item.Quality, item.Durability);
                    }

                    var commandList = await _commandListRepository.GetListByVipGiftIdAsync(vipGift.Id);
                    foreach (var item in commandList)
                    {
                        Utils.ExecuteConsoleCommand(FormatCmd(item.Command, onlinePlayer, vipGift), item.InMainThread);
                    }

                    vipGift.ClaimState = true;
                    vipGift.TotalClaimCount++;
                    vipGift.LastClaimAt = DateTime.Now;
                    await _vipGiftRepository.UpdateAsync(vipGift);

                    SendMessageToPlayer(playerId, FormatCmd(Settings.ClaimSuccessTip, onlinePlayer, vipGift));

                    CustomLogger.Info("PlayerId: {0}, playerName: {1}, {1} claimed: {2}.", playerId, onlinePlayer.PlayerName, vipGift.Name);
                }

                return true;
            }
            return false;
        }

        private static string FormatCmd(string message, OnlinePlayer player, T_VipGift vipGift)
        {
            string result = StringTemplate.Render(message, new VipGiftVariables()
            {
                EntityId = player.EntityId,
                PlatformId = player.PlatformId,
                PlayerName = player.PlayerName,
                GiftName = vipGift.Name,
                TotalClaimCount = vipGift.TotalClaimCount,
                GiftDescription = vipGift.Description,
            });
            return result;
        }
    }
}