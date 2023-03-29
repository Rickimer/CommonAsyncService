using CommonAsyncService.BLL;
using CommonAsyncService.BLL.DTO;
using EmailService;
using EmailService.Shared;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Text;

namespace MultyHealthCheck
{
    public class RequestHealthCheck : IHealthCheck
    {
        private readonly IMonitorSystems _monitorSystems;
        private readonly IBllMonitorSystemsInfo _bllMonitorSystemsInfo;        

        public RequestHealthCheck(IMonitorSystems monitorSystems
            , IBllMonitorSystemsInfo bllMonitorSystemsInfo
            )
        {            
            _monitorSystems = monitorSystems;
            _bllMonitorSystemsInfo = bllMonitorSystemsInfo;            
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var report = await _monitorSystems.CheckAllSystems();
            var Result = new StringBuilder();            

            foreach (var key in report.CommonRezult.Keys)
            {
                if (report.CommonRezult[key] == HealthCheckRezultsDto.Success)
                {
                    Result.Append($"Success connect to {key}. ");
                }
                else
                {
                    Result.Append($"Error connect to {key}. ");
                }
            }            

            return HealthCheckResult.Healthy($"{Result}");
        }
    }
}
