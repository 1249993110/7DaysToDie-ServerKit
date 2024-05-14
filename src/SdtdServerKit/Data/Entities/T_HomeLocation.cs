using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 家园位置
    /// </summary>
    public class T_HomeLocation
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
        /// Home名称
        /// </summary>
        public string HomeName { get; set; } = null!;

        /// <summary>
        /// 三维坐标
        /// </summary>
        public string Position { get; set; } = null!;
    }
}