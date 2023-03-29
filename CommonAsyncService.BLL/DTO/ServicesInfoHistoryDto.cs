namespace CommonAsyncService.BLL.DTO
{
    public class ServicesInfoHistoryDto
    {
        public ulong Id { get; set; }
        public DateTime Created { get; set; }
        public ConsumingServicesDto ConsumingService { get; set; }
        public string? Info { get; set; }
    }
}
