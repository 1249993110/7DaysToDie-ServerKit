using IceCoffee.SimpleCRUD.Dtos;

namespace SdtdServerKit.Data.Dtos
{
    public class DateTimeQueryDto : PaginationQueryDto
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
