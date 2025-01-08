using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;

namespace SdtdServerKit.Data.Repositories
{
    /// <summary>
    /// Represents a Cd Key Command repository.
    /// </summary>
    public class CdKeyCommandRepository : DefaultRepository<CdKeyCommand>, ICdKeyCommandRepository
    {
    }
}