using CommonAsyncService.DAL.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonAsyncService.DAL.Data.Models
{
    public class StoryMailProcessing : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        public DateTime Created { get; set; }
        public uint UserId { get; set; }
        public ConsumingServices ConsumingService { get; set; }
        public SendRezults Rezult { get; set; }
    }
}
