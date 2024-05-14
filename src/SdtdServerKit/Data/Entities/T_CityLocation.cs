using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    /// <summary>
    /// 城市位置
    /// </summary>
    public class T_CityLocation
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
        /// 城市名称
        /// </summary>
        public string CityName { get; set; } = null!;

        /// <summary>
        /// 传送需要积分
        /// </summary>
        public int PointsRequired { get; set; }

        /// <summary>
        /// 三维坐标
        /// </summary>
        public string Position { get; set; } = null!;
    }
}