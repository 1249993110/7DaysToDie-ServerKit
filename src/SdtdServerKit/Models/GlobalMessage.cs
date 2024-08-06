namespace SdtdServerKit.Models
{
    /// <summary>
    /// 全局消息
    /// </summary>
    public class GlobalMessage
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// 发送者昵称
        /// </summary>
        public string? SenderName { get; set; }
    }
}