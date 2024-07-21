using IceCoffee.SimpleCRUD;
using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Entities;

namespace SdtdServerKit.Data.IRepositories
{
    /// <summary>
    /// 物品清单仓储
    /// </summary>
    public interface IItemListRepository : IRepository<T_ItemList>
    {
        /// <summary>
        /// 根据商品Id获取物品清单
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        Task<IEnumerable<T_ItemList>> GetListByGoodsIdAsync(int goodsId);

        /// <summary>
        /// 分页获取物品清单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<PagedDto<T_ItemList>> GetPagedListAsync(PaginationQueryDto dto);
    }
}