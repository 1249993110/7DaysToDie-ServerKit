using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
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
                var goodsList = await _goodsRepository.GetAllOrderByPriceAsync();
                if (goodsList.Any() == false)
                {
                    SendMessageToPlayer(playerId, Settings.NoGoods);
                }
                else
                {
                    int index = 0;
                    foreach (var item in goodsList)
                    {
                        index++;
                        SendMessageToPlayer(playerId, FormatCmd(Settings.GoodsItemTip, onlinePlayer, item, index));
                    }
                }

                return true;
            }
            else if(message.StartsWith(Settings.BuyCmdPrefix + ConfigManager.GlobalSettings.ChatCommandSeparator, StringComparison.OrdinalIgnoreCase))
            {
                string goodsName = message.Substring(Settings.BuyCmdPrefix.Length + ConfigManager.GlobalSettings.ChatCommandSeparator.Length);
                var goods = await _goodsRepository.GetByNameAsync(goodsName);
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

                        string[] cmds = goods.ExecuteCommands.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < cmds.Length; i++)
                        {
                            Utils.ExecuteConsoleCommand(FormatCmd(cmds[i], onlinePlayer, goods, i + 1), goods.InMainThread);
                        }

                        SendMessageToPlayer(playerId, FormatCmd(Settings.BuySuccessTip, onlinePlayer, goods));

                        CustomLogger.Info("PlayerId: {0}, playerName: {1}, {1} bought: {2}.", playerId, onlinePlayer.PlayerName, goods.Name);
                    }

                    return true;
                }
            }

            return false;
        }

        private static string FormatCmd(string message, OnlinePlayer player, T_Goods goods, int serialNumber = 0)
        {
            string result = StringTemplate.Render(message, new GameStoreVariables()
            {
                EntityId = player.EntityId,
                PlatformId = player.PlatformId,
                PlayerName = player.PlayerName,
                GoodsName = goods.Name,
                Price = goods.Price,
                SerialNumber = serialNumber,
            });
            return result;
        }
    }
}