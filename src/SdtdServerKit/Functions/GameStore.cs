using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.Managers;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// 游戏商店
    /// </summary>
    public class GameStore : FunctionBase<GameStoreSettings>
    {
        private readonly IGoodsRepository _goodsRepository;
        private readonly IPointsInfoRepository _pointsInfoRepository;
        /// <inheritdoc/>
        public GameStore(IPointsInfoRepository pointsInfoRepository, IGoodsRepository goodsRepository)
        {
            _pointsInfoRepository = pointsInfoRepository;
            _goodsRepository = goodsRepository;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnChatCmd(string message, OnlinePlayer onlinePlayer)
        {
            if (string.Equals(message, Settings.QueryListCmd, StringComparison.OrdinalIgnoreCase))
            {
                string playerId = onlinePlayer.CrossplatformId;
                var goodsList = await _goodsRepository.GetAllOrderByIdAsync();
                if (goodsList.Any() == false)
                {
                    SendMessageToPlayer(playerId, Settings.NoGoods);
                }
                else
                {
                    foreach (var item in goodsList)
                    {
                        SendMessageToPlayer(playerId, FormatCmd(Settings.GoodsItemTip, onlinePlayer, item));
                    }
                }

                return true;
            }
            else if (message.StartsWith(Settings.BuyCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                int goodsId = message.Substring(Settings.BuyCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length).ToInt(-1);
                var goods = await _goodsRepository.GetByIdAsync(goodsId);
                if (goods == null)
                {
                    return false;
                }
                else
                {
                    string playerId = onlinePlayer.CrossplatformId;

                    int points = await _pointsInfoRepository.GetPointsByIdAsync(playerId);
                    if (points < goods.Price)
                    {
                        SendMessageToPlayer(playerId, FormatCmd(Settings.PointsNotEnoughTip, onlinePlayer, goods));
                    }
                    else
                    {
                        await _pointsInfoRepository.ChangePointsAsync(playerId, -goods.Price);

                        if(goods.ContentType == GoodsContentType.Item)
                        {
                            var item = JsonConvert.DeserializeObject<SdtdServerKit.Shared.Models.ItemStack>(goods.Content, ModApi.JsonSerializerSettings);
                            if (item != null)
                            {
                                Utils.GiveItem(playerId, item.ItemName, item.Count, item.Quality, item.Durability);
                            }
                        }
                        else if(goods.ContentType == GoodsContentType.Command)
                        {
                            string[]? cmds = JsonConvert.DeserializeObject<string[]>(goods.Content, ModApi.JsonSerializerSettings);
                            if(cmds != null)
                            {
                                foreach (var cmd in cmds)
                                {
                                    if(string.IsNullOrEmpty(cmd) == false)
                                    {
                                        Utils.ExecuteConsoleCommand(FormatCmd(cmd, onlinePlayer, goods), goods.InMainThread);
                                    }
                                }
                            }
                        }

                        SendMessageToPlayer(playerId, FormatCmd(Settings.BuySuccessTip, onlinePlayer, goods));

                        CustomLogger.Info("PlayerId: {0}, playerName: {1}, {1} bought: {2}.", playerId, onlinePlayer.PlayerName, goods.Name);
                    }

                    return true;
                }
            }

            return false;
        }

        private static string FormatCmd(string message, OnlinePlayer player, T_Goods goods)
        {
            string result = StringTemplate.Render(message, new GameStoreVariables()
            {
                EntityId = player.EntityId,
                PlatformId = player.PlatformId,
                PlayerName = player.PlayerName,
                GoodsId = goods.Id,
                GoodsName = goods.Name,
                Price = goods.Price,
            });
            return result;
        }
    }
}