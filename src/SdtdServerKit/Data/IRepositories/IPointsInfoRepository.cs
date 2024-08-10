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
        /// <summary>
        /// 根据玩家ID异步获取积分
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <returns>积分</returns>
        Task<int> GetPointsByIdAsync(string playerId);

        /// <summary>
        /// 根据玩家ID列表异步获取积分字典
        /// </summary>
        /// <param name="playerIds">玩家ID列表</param>
        /// <returns>积分字典</returns>
        Task<IReadOnlyDictionary<string, int>> GetPointsByIdsAsync(IEnumerable<string> playerIds);

        /// <summary>
        /// 根据玩家ID异步修改积分
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="points">积分</param>
        /// <returns>修改后的积分</returns>
        Task<int> ChangePointsAsync(string playerId, int points);

        /// <summary>
        /// 异步获取分页的积分信息
        /// </summary>
        /// <param name="dto">分页查询参数</param>
        /// <returns>分页的积分信息</returns>
        Task<PagedDto<T_PointsInfo>> GetPagedListAsync(PaginationQueryDto dto);

        /// <summary>
        /// 异步重置积分
        /// </summary>
        /// <returns>重置的积分数量</returns>
        Task<int> ResetPointsAsync();

        /// <summary>
        /// 异步重置签到状态
        /// </summary>
        /// <returns>重置的签到状态数量</returns>
        Task<int> ResetSignInAsync();
    }
}