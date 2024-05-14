namespace SdtdServerKit.Shared.Models
{
    public class PointsInfo
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 拥有积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 上次签到天数
        /// </summary>
        public int LastSignInDays { get; set; }
    }
}
