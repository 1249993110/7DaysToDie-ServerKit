using System.ComponentModel;

namespace SdtdServerKit.Models
{
    public enum ChatRecordQueryOrder
    {
        /// <summary>
        /// 
        /// </summary>
        CreatedAt,
    }

    /// <summary>
    /// 日期时间查询
    /// </summary>
    public class DateTimeQuery : PaginationQuery<ChatRecordQueryOrder>
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }
    }
}