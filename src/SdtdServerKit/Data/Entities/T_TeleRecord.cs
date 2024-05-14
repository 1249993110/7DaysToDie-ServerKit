using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 传送记录
    /// </summary>
    public class T_TeleRecord
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
        /// 玩家Id
        /// </summary>
        public string PlayerId { get; set; } = null!;

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; } = null!;

        /// <summary>
        /// 目标类型
        /// </summary>
        public string TargetType { get; set; } = null!;

        /// <summary>
        /// 目标名称
        /// </summary>
        public string TargetName { get; set; } = null!;

        /// <summary>
        /// 源坐标, 空格分割
        /// </summary>
        public string OriginPosition { get; set; } = null!;

        /// <summary>
        /// 目的坐标, 空格分割
        /// </summary>
        public string TargetPosition { get; set; } = null!;
    }
}