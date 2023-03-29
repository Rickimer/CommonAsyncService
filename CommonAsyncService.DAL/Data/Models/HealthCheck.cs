using CommonAsyncService.DAL.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonAsyncService.DAL.Data.Models
{
    public class HealthCheck : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        public DateTime Created { get; set; }        
        public ConsumingServices ConsumingService { get; set; }
        public HealthCheckRezults Rezult { get; set; }
    }
}
