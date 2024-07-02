using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.Text;

namespace SdtdServerKit.Data.Repositories
{
    public class PointsInfoRepository : DefaultRepository<T_PointsInfo>, IPointsInfoRepository
    {
        public Task<int> GetPointsByIdAsync(string playerId)
        {
            string sql = "SELECT Points FROM T_PointsInfo WHERE Id=@PlayerId";
            return base.ExecuteScalarAsync<int>(sql, new { PlayerId = playerId });
        }

        public Task ChangePointsAsync(string playerId, int points)
        {
            string sql = "UPDATE T_PointsInfo SET Points=Points+@Points WHERE Id=@PlayerId";
            return base.ExecuteAsync(sql, new { PlayerId = playerId, Points = points });
        }

        public Task<PagedDto<T_PointsInfo>> GetPagedListAsync(PaginationQueryDto dto)
        {
            var whereClauseSB = new StringBuilder("1=1");

            if (string.IsNullOrEmpty(dto.Keyword) == false)
            {
                whereClauseSB.Append(" AND (Id=@Keyword OR PlayerName LIKE '%'||@Keyword||'%')");
            }

            //string orderByClause;
            //if (string.IsNullOrEmpty(dto.Order) == false && typeof(T_ChatRecord).GetProperty(dto.Order) != null)
            //{
            //    orderByClause = dto.Order + (dto.Desc ? "DESC" : "ASC");
            //}
            //else
            //{
            //    orderByClause = "CreatedAt " + (dto.Desc ? "DESC" : "ASC");
            //}

            var param = new { Keyword = dto.Keyword };
            return GetPagedListAsync(dto.PageNumber, dto.PageSize, whereClauseSB.ToString(), orderByClause: "PlayerName ASC", param);
        }
    }
}