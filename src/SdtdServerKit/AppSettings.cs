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
        public string UserName { get; set; } = null!;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null!;
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string WebUrl { get; set; } = null!;
        /// <summary>
        /// WebSocket端口
        /// </summary>
        public int WebSocketPort { get; set; }
        /// <summary>
        /// WebSocket地址
        /// </summary>
        public string WebSocketUrl { get; set; } = null!;
        /// <summary>
        /// AccessToken到期时间
        /// </summary>
        public int AccessTokenExpireTime { get; set; }
        /// <summary>
        /// 数据库路径
        /// </summary>
        public string DatabasePath { get; set; } = null!;
        /// <summary>
        /// 文件浏览器端口
        /// </summary>
        public int FileBrowserPort { get; set; }
    }
}