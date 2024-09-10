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
        /// 发送者名称
        /// </summary>
        public string? SenderName { get; set; }
    }
}