namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 商品
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 执行命令
        /// </summary>
        public string ExecuteCommands { get; set; } = null!;

        /// <summary>
        /// 在主线程执行
        /// </summary>
        public bool InMainThread { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
    }
}
