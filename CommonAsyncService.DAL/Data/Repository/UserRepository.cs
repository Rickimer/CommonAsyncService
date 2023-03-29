using CommonAsyncService.DAL.Data.Models;

namespace CommonAsyncService.DAL.Data.Repository
{
    public class UserRepository : GeneralRepository<User, CommonServiceDBContext>
    {
        public UserRepository(CommonServiceDBContext context) : base(context)
        {
        }
    }
}
