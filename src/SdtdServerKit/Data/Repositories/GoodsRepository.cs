using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;

namespace SdtdServerKit.Data.Repositories
{
    /// <summary>
    /// 商品仓储
    /// </summary>
    public class GoodsRepository : DefaultRepository<T_Goods>, IGoodsRepository
    {
        /// <inheritdoc/>
        public Task<T_Goods?> GetByNameAsync(string name)
        {
            return base.GetFirstOrDefaultAsync("Name=@Name", param: new { Name = name });
        }
        /// <inheritdoc/>
        public Task<IEnumerable<T_Goods>> GetAllOrderByPriceAsync(bool isDesc = true)
        {
            return base.GetListAsync(orderByClause: "Price " + (isDesc ? "DESC" : "ASC"));
        }
    }
}