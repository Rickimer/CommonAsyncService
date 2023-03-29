using CommonAsyncService.BLL.DTO;

namespace CommonAsyncService.BLL
{
    public interface IBllMonitorSystemsInfo
    {
        Task<ulong> AddHealthCheck(HealthsCheckDto dto);        
        Task<ulong> AddServicesInfoHistory(ServicesInfoHistoryDto dto);
        Task<bool> NeedCheckInfo(ConsumingServicesDto consumingServicesDto);
    }
}
