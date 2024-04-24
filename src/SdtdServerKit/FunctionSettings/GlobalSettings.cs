namespace SdtdServerKit.FunctionSettings
{
    public class GlobalSettings : ISettings
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 聊天命令前缀
        /// </summary>
        public string ChatCommandPrefix { get; set; }

        /// <summary>
        /// 聊天命令分隔符
        /// </summary>
        public string ChatCommandSeparator { get; set; }

        /// <summary>
        /// 聊天消息错误提示
        /// </summary>
        public string HandleChatMessageError { get; set; }

        bool ISettings.IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}