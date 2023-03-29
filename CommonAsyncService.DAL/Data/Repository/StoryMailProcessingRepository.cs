using CommonAsyncService.DAL.Data.Models;

namespace CommonAsyncService.DAL.Data.Repository
{
    /// <summary>
    /// email sending history
    /// </summary>
    public class StoryMailProcessingRepository : GeneralRepository<StoryMailProcessing, CommonServiceDBContext>
    {
        public StoryMailProcessingRepository(CommonServiceDBContext context) : base(context)
        {
        }
    }
}
