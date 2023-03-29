namespace CommonAsyncService.BLL.Shared
{
    public class CheckSystemsOptions
    {
        public int HealthCheckPeriodSeconds { get; set; }
        public int MonitoringSystemsPeriodSeconds { get; set; }
        public string AuthServer { get; set; } = String.Empty;
        public string TodoService { get; set; } = String.Empty;
    }
}
