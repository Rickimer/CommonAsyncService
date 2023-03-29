namespace CommonAsyncService.BLL.DTO
{
    public class HealthsCheckDto
    {
        public ulong Id { get; set; }
        public DateTime Created { get; set; }
        public ConsumingServicesDto ConsumingService { get; set; }
        public HealthCheckRezultsDto Rezult { get; set; }
    }
}
