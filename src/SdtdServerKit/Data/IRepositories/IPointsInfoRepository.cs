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
        Task<Dictionary<string, int>> GetPointsByIdsAsync(IEnumerable<string> playerIds);

        Task<int> ChangePointsAsync(string playerId, int points);

        Task<PagedDto<T_PointsInfo>> GetPagedListAsync(PaginationQueryDto dto);

        Task<int> ResetPointsAsync();
        Task<int> ResetSignInAsync();
    }
}