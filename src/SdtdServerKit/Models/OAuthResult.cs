namespace SdtdServerKit.Models
{
    /// <summary>
    /// OAuth Result
    /// </summary>
    public class OAuthResult
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Token Type
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = null!;

        /// <summary>
        /// Expires In
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}