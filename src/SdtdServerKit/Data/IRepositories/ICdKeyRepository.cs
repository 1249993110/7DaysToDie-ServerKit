using IceCoffee.SimpleCRUD;
using SdtdServerKit.Data.Entities;

namespace SdtdServerKit.Data.IRepositories
{
    /// <summary>
    /// Represents a Cd Key repository interface.
    /// </summary>
    public interface ICdKeyRepository : IRepository<CdKey>
    {
        /// <summary>
        /// Get a Cd Key by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<CdKey?> GetByKeyAsync(string key);
    }
}