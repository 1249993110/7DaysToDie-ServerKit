namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 在线玩家
    /// </summary>
    public class OnlinePlayer : IPlayer
    {
        /// <summary>
        /// 实体Id
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// 平台Id
        /// </summary>
        public string PlatformId { get; set; } = null!;

        /// <summary>
        /// 跨平台Id
        /// </summary>
        public string CrossplatformId { get; set; } = null!;

        /// <summary>
        /// 名称
        /// </summary>
        public string PlayerName { get; set; } = null!;

        /// <summary>
        /// IP
        /// </summary>
        public string Ip { get; set; } = null!;

        /// <summary>
        /// Ping
        /// </summary>
        public int Ping { get; set; }
    }
}