using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 积分信息
    /// </summary>
    public class T_PointsInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        [PrimaryKey]
        [Column("Id")]
        public string PlayerId { get; set; } = null!;

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 拥有积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 上次签到天数
        /// </summary>
        public int LastSignInDays { get; set; }
    }
}
