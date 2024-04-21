using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;

namespace SdtdServerKit.Data.Repositories
{
    public class ChatRecordRepository : DefaultRepository<T_ChatRecord>, IChatRecordRepository
    {
        public ChatRecordRepository()
        {
        }
    }
}