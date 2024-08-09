using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.Text;
using Webserver.WebAPI.APIs.WorldState;

namespace SdtdServerKit.Data.Repositories
{
    public class PointsInfoRepository : DefaultRepository<T_PointsInfo>, IPointsInfoRepository
    {
        public Task<int> GetPointsByIdAsync(string playerId)
        {
            string sql = $"SELECT Points FROM {SqlGenerator.TableName} WHERE Id=@PlayerId";
            return base.ExecuteScalarAsync<int>(sql, new { PlayerId = playerId });
        }

        public Task<int> ChangePointsAsync(string playerId, int points)
        {
            var entity = base.GetById(playerId);
            if(entity == null)
            {
                entity = new T_PointsInfo()
                {
                    CreatedAt = DateTime.Now,
                    Id = playerId,
                    LastSignInAt = null,
                    PlayerName = string.Empty,
                    Points = points
                };
                return base.InsertAsync(entity);
            }
            else
            {
                string sql = $"UPDATE {SqlGenerator.TableName} SET Points=Points+@Points WHERE Id=@PlayerId";
                return base.ExecuteAsync(sql, new { PlayerId = playerId, Points = points });
            }
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

        public Task<int> ResetPointsAsync()
        {
            string sql = $"UPDATE {SqlGenerator.TableName} SET Points=0";
            return base.ExecuteAsync(sql, useTransaction: true);
        }

        public Task<int> ResetSignInAsync()
        {
            string sql = $"UPDATE {SqlGenerator.TableName} SET LastSignInAt=NULL";
            return base.ExecuteAsync(sql, useTransaction: true);
        }

        public Task<IEnumerable<int>> GetPointsByIdsAsync(IEnumerable<string> playerIds)
        {
            string sql = $"SELECT Points FROM {SqlGenerator.TableName} WHERE Id IN @PlayerIds";
            return base.ExecuteQueryAsync<int>(sql, new { PlayerIds = playerIds });
        }
    }
}