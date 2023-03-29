using CommonAsyncService.BLL.DTO;

namespace MultyHealthCheck
{
    public interface IMonitorSystems
    {
        Task<CommonRezultDto> CheckAllSystems();
        Task<HealthCheckRezultsDto> ChecSpecificSystem(bool isFullReport, ConsumingServicesDto consumingServicesDto);
        Task<HealthCheckRezultsDto> CheckTodoSystem(bool isFullReport);
        Task<HealthCheckRezultsDto> CheckGateAPI(bool isFullReport);
    }
}
