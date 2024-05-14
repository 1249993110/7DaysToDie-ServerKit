using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 商品
    /// </summary>
    public class T_Goods
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        [PrimaryKey, IgnoreUpdate, IgnoreInsert]
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
        /// 执行命令
        /// </summary>
        public string ExecuteCommands { get; set; } = null!;

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