using CommonAsyncService.DAL.Data.Enums;

namespace CommonAsyncService.DAL.Data.Models
{
    /// <summary>
    /// Full report about service work history
    /// </summary>
    public class ServicesInfoHistory : IEntity
    {
        public ulong Id { get; set; }
        public DateTime Created { get; set; }
        public ConsumingServices ConsumingService { get; set; }
        public string? Info { get; set; }
    }
}
