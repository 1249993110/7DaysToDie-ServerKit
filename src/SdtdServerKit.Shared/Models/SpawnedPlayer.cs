namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 生成的玩家
    /// </summary>
    public class SpawnedPlayer : OnlinePlayer
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 生成类型
        /// </summary>
        public RespawnType RespawnType { get; set; }
    }
}