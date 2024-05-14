namespace SdtdServerKit.Shared.Models
{
    public class HomeLocation
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        public string PlayerId { get; set; } = null!;

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; } = null!;

        /// <summary>
        /// Home名称
        /// </summary>
        public string HomeName { get; set; } = null!;

        /// <summary>
        /// 三维坐标
        /// </summary>
        public string Position { get; set; } = null!;
    }
}
