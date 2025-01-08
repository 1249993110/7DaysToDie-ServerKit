using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;

namespace SdtdServerKit.Data.Repositories
{
    /// <summary>
    /// Represents a Cd Key repository.
    /// </summary>
    public class CdKeyRepository : DefaultRepository<CdKey>, ICdKeyRepository
    {
        public Task<CdKey?> GetByKeyAsync(string key)
        {
            return base.GetFirstOrDefaultAsync("[Key]=@Key", param: new { Key = key });
        }
    }
}