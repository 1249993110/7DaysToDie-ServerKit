namespace SdtdServerKit
{
    /// <summary>
    /// AppSettings
    /// </summary>
    public class AppSettings
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string WebUrl { get; set; } = null!;
        public int WebSocketPort { get; set; }
        public string WebSocketUrl { get; set; } = null!;
        public int AccessTokenExpireTime { get; set; }

        public string DatabasePath { get; set; } = null!;
    }
}