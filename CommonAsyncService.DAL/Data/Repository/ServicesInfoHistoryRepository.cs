using CommonAsyncService.DAL.Data.Models;

namespace CommonAsyncService.DAL.Data.Repository
{    
    public class ServicesInfoHistoryRepository : GeneralRepository<ServicesInfoHistory, CommonServiceDBContext>
    {
        public ServicesInfoHistoryRepository(CommonServiceDBContext context) : base(context)
        {
        }
    }
}
