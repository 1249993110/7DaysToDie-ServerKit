using IceCoffee.SimpleCRUD.Dtos;
using SdtdServerKit.Data.Dtos;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.Text;

namespace SdtdServerKit.Data.Repositories
{
    public class ChatRecordRepository : DefaultRepository<T_ChatRecord>, IChatRecordRepository
    {
        public Task<PagedDto<T_ChatRecord>> GetPagedListAsync(DateTimeQueryDto dto)
        {
            var whereClauseSB = new StringBuilder("1=1");

            if (dto.StartDateTime.HasValue)
            {
                whereClauseSB.Append($" AND CreatedAt>={dto.StartDateTime.Value:s}");
            }

            if (dto.EndDateTime.HasValue)
            {
                whereClauseSB.Append($" AND CreatedAt<={dto.EndDateTime.Value:s}");
            }

            if (string.IsNullOrEmpty(dto.Keyword) == false)
            {
                whereClauseSB.Append(" AND (EntityId=@Keyword OR PlayerId=@Keyword OR SenderName LIKE '%'||@Keyword||'%' OR Message LIKE '%'||@Keyword||'%')");
            }

            string orderByClause = dto.Order + (dto.Desc ? " DESC" : " ASC");

            var param = new { Keyword = dto.Keyword };
            return base.GetPagedListAsync(dto.PageNumber, dto.PageSize, whereClauseSB.ToString(), orderByClause, param);
        }
    }
}