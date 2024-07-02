using SdtdServerKit.Shared.Constants;

namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 商品
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// 内容类型
        /// </summary>
        public GoodsContentType ContentType { get; set; }

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
