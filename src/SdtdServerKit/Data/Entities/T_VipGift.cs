using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 商品
    /// </summary>
    [Table("T_VipGift")]
    public class T_VipGift
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

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
        /// 领取状态, true: 已领取, false: 未领取
        /// </summary>
        public bool ClaimState { get; set; }

        /// <summary>
        /// 总领取次数
        /// </summary>
        public int TotalClaimCount { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string? Description { get; set; }
    }
}