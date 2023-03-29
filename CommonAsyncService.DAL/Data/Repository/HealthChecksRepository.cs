using CommonAsyncService.DAL.Data.Models;

namespace CommonAsyncService.DAL.Data.Repository
{
    public class HealthChecksRepository : GeneralRepository<HealthCheck, CommonServiceDBContext>
    {
        public HealthChecksRepository(CommonServiceDBContext context) : base(context)
        {
        }
    }
}
