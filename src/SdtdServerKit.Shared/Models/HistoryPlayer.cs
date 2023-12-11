namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// History Player
    /// </summary>
    public class HistoryPlayer
    {
        /// <summary>
        /// 平台Id
        /// </summary>
        public string PlatformId { get; set; } = null!;

        /// <summary>
        /// 跨平台Id
        /// </summary>
        public string CrossplatformId { get; set; } = null!;

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; } = null!;

        /// <summary>
        /// 坐标
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 最后登录
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}
