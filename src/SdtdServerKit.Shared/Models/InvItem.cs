namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 库存项目
    /// </summary>
    public class InvItem
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 质量
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 图标颜色
        /// </summary>
        public string IconColor { get; set; }

        /// <summary>
        /// 质量颜色
        /// </summary>
        public string? QualityColor { get; set; }

        /// <summary>
        /// 最大使用次数
        /// </summary>
        public int MaxUseTimes { get; set; }

        /// <summary>
        /// 已使用次数
        /// </summary>
        public float UseTimes { get; set; }

        /// <summary>
        /// 部件
        /// </summary>
        public InvItem?[] Parts { get; set; }
    }
}
