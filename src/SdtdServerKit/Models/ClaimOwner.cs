namespace SdtdServerKit.Models
{
    /// <summary>
    /// 领地石所有者
    /// </summary>
    public class ClaimOwner : PlayerBase
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool ClaimActive { get; set; }

        /// <summary>
        /// 领地石坐标集合
        /// </summary>
        public IEnumerable<Position> ClaimPositions { get; set; }
    }
}