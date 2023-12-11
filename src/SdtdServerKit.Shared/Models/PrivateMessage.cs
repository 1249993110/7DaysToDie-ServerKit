namespace SdtdServerKit.Shared.Models
{
    public class PrivateMessage : GlobalMessage
    {
        /// <summary>
        /// 目标玩家的Id或昵称
        /// </summary>
        public string TargetPlayerIdOrName { get; set; } = null!;
    }
}