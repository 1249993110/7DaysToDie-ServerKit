using System.ComponentModel;

namespace SdtdServerKit.Models
{
    /// <summary>
    /// 给与项目
    /// </summary>
    public class GiveItem
    {
        /// <summary>
        /// 数量
        /// </summary>
        [DefaultValue(1)]
        public int Count { get; set; } = 1;

        /// <summary>
        /// 耐久度百分比
        /// </summary>
        public int? Durability { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; } = null!;

        /// <summary>
        /// 质量
        /// </summary>
        public int? Quality { get; set; }

        /// <summary>
        /// 目标玩家的Id或昵称
        /// </summary>
        public string TargetPlayerIdOrName { get; set; } = null!;
    }
}