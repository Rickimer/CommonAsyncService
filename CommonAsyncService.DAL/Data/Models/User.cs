namespace CommonAsyncService.DAL.Data.Models
{
    public class User : IEntity
    {
        public ulong Id { get; set; }
        public uint UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }
}
