namespace SdtdServerKit
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public required string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public required string Password { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public required string WebUrl { get; set; }
        /// <summary>
        /// WebSocket端口
        /// </summary>
        public int WebSocketPort { get; set; }
        /// <summary>
        /// WebSocket地址
        /// </summary>
        public required string WebSocketUrl { get; set; }
        /// <summary>
        /// AccessToken到期时间
        /// </summary>
        public int AccessTokenExpireTime { get; set; }
        /// <summary>
        /// 数据库路径
        /// </summary>
        public required string DatabasePath { get; set; }
        /// <summary>
        /// 服务器配置文件名
        /// </summary>
        public required string ServerSettingsFileName { get; set; }
    }
}