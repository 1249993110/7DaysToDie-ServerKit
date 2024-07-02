using IceCoffee.SimpleCRUD.OptionalAttributes;
using SdtdServerKit.Data.Dtos;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 商品
    /// </summary>
    [Table("T_Goods_v1")]
    public class T_Goods
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [IgnoreUpdate]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 名称
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
        /// 是否在主线程执行
        /// </summary>
        public bool InMainThread { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
    }
}