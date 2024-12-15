using IceCoffee.SimpleCRUD;
using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Entities;

namespace SdtdServerKit.Data.IRepositories
{
    /// <summary>
    /// 命令清单仓储
    /// </summary>
    public interface ICommandListRepository : IRepository<T_CommandList>
    {
        /// <summary>
        /// 根据商品Id获取命令清单
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        Task<IEnumerable<T_CommandList>> GetListByGoodsIdAsync(int goodsId);

        /// <summary>
        /// 根据VIP礼包Id获取命令清单
        /// </summary>
        /// <param name="vipGiftId"></param>
        /// <returns></returns>
        Task<IEnumerable<T_CommandList>> GetListByVipGiftIdAsync(string vipGiftId);

        /// <summary>
        /// 根据任务调度Id获取命令清单
        /// </summary>
        /// <param name="taskScheduleId"></param>
        /// <returns></returns>
        Task<IEnumerable<T_CommandList>> GetListByTaskScheduleIdAsync(int taskScheduleId);

        /// <summary>
        /// 分页获取命令清单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<PagedDto<T_CommandList>> GetPagedListAsync(PaginationQueryDto dto);
    }
}