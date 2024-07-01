using IceCoffee.SimpleCRUD;
using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Entities;

namespace SdtdServerKit.Data.IRepositories
{
    /// <summary>
    /// 积分信息仓储接口
    /// </summary>
    public interface IPointsInfoRepository : IRepository<T_PointsInfo>
    {
        Task<int> GetPointsByIdAsync(string playerId);

        Task ChangePointsAsync(string playerId, int points);

        Task<PagedDto<T_PointsInfo>> GetPagedListAsync(PaginationQueryDto dto);

        /// <summary>
        /// 重置签到天数
        /// </summary>
        /// <returns></returns>
        Task<int> ResetLastSignInDaysAsync();
    }
}