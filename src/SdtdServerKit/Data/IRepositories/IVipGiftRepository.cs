using IceCoffee.SimpleCRUD;
using SdtdServerKit.Data.Entities;

namespace SdtdServerKit.Data.IRepositories
{
    /// <summary>
    /// 礼品仓储
    /// </summary>
    public interface IVipGiftRepository : IRepository<T_VipGift>
    {
        /// <summary>
        /// 重置领取状态
        /// </summary>
        /// <returns></returns>
        Task<int> ResetClaimStateAsync();
    }
}