using System.ComponentModel;

namespace SdtdServerKit.Shared.Models
{
    /// <summary>
    /// 日期时间查询
    /// </summary>
    public class DateTimeQuery
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;


        /// <summary>
        /// 每页数量, 值小于 0 时返回所有记录
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string? Keyword { get; set; }
    }
}