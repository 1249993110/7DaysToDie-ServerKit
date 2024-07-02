using IceCoffee.SimpleCRUD;
using SdtdServerKit.Data.Entities;

namespace SdtdServerKit.Data.IRepositories
{
    /// <summary>
    /// 商品仓储
    /// </summary>
    public interface IGoodsRepository : IRepository<T_Goods>
    {
        /// <summary>
        /// 根据名称获取商品
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<T_Goods?> GetByNameAsync(string name);

        /// <summary>
        /// 获取所有商品按Id升序排序
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T_Goods>> GetAllOrderByIdAsync();
    }
}