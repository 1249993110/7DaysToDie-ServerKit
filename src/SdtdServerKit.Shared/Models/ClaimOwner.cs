namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 领地石所有者
    /// </summary>
    public class ClaimOwner
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool ClaimActive { get; set; }

        /// <summary>
        /// 领地石坐标集合
        /// </summary>
        public IEnumerable<Position> ClaimPositions { get; set; }

        /// <summary>
        /// 平台Id
        /// </summary>
        public string PlatformId { get; set; }

        /// <summary>
        /// 跨平台Id
        /// </summary>
        public string CrossplatformId { get; set; }

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }
    }
}