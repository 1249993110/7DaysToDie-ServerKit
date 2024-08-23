namespace SdtdServerKit.Constants
{
    /// <summary>
    /// Locales
    /// </summary>
    public struct Locales
    {
        /// <summary>
        /// English
        /// </summary>
        public const string EnUs = "en";

        /// <summary>
        /// 简体中文
        /// </summary>
        public const string ZhCn = "zh";

        /// <summary>
        /// 繁體中文
        /// </summary>
        public const string ZhTw = "tw";

        /// <summary>
        /// Get locale by language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string Get(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return EnUs;
                case Language.Schinese:
                    return ZhCn;
                case Language.Tchinese:
                    return ZhTw;
                default:
                    return EnUs;
            }
        }

        /// <summary>
        /// Get locale by language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string Get(string language)
        {
            if (string.Equals(language, Language.English.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return EnUs;
            }

            if (string.Equals(language, Language.Schinese.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ZhCn;
            }

            if (string.Equals(language, Language.Tchinese.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ZhTw;
            }

            return EnUs;
        }
    }
}
