namespace SdtdServerKit.Shared.Constants
{
    /// <summary>
    /// 商品内容类型
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GoodsContentType
    {
        /// <summary>
        /// 物品
        /// </summary>
        Item,

        /// <summary>
        /// 命令
        /// </summary>
        Command
    }
}
