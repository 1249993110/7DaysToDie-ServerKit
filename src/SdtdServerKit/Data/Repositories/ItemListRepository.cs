using IceCoffee.SimpleCRUD.Dtos;
using IceCoffee.SimpleCRUD.SqlGenerators;
using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using System.Text;

namespace SdtdServerKit.Data.Repositories
{
    /// <summary>
    /// 物品清单仓储
    /// </summary>
    public class ItemListRepository : DefaultRepository<T_ItemList>, IItemListRepository
    {
        /// <inheritdoc/>
        public Task<IEnumerable<T_ItemList>> GetListByGoodsIdAsync(int goodsId)
        {
            string whereClause = "Id IN (SELECT ItemId FROM T_GoodsItem WHERE GoodsId=@GoodsId)";
            return base.GetListAsync(whereClause, param: new { GoodsId = goodsId });
        }

        /// <inheritdoc/>
        public Task<IEnumerable<T_ItemList>> GetListByVipGiftIdAsync(string vipGiftId)
        {
            string tableName = GetSqlGenerator<T_VipGiftItem>().TableName;
            string whereClause = $"Id IN (SELECT ItemId FROM {tableName} WHERE VipGiftId=@VipGiftId)";
            return base.GetListAsync(whereClause, param: new { VipGiftId = vipGiftId });
        }

        /// <inheritdoc/>
        public Task<PagedDto<T_ItemList>> GetPagedListAsync(PaginationQueryDto dto)
        {
            var whereClauseSB = new StringBuilder("1=1");

            if (string.IsNullOrEmpty(dto.Keyword) == false)
            {
                whereClauseSB.Append(" AND (Id=@Keyword OR ItemName LIKE '%'||@Keyword||'%' OR Description LIKE '%'||@Keyword||'%')");
            }

            var param = new { Keyword = dto.Keyword };
            return GetPagedListAsync(dto.PageNumber, dto.PageSize, whereClauseSB.ToString(), orderByClause: "Id DESC", param);
        }
    }
}