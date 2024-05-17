namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 聊天消息
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// 聊天类型
        /// </summary>
        public ChatType ChatType { get; set; }

        /// <summary>
        /// 实体Id
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// 玩家Id
        /// </summary>
        public string? PlayerId { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 发送者名称
        /// </summary>
        public string SenderName { get; set; }
    }
}