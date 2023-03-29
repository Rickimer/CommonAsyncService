using AutoMapper;
using CommonAsyncService.BLL;
using CommonAsyncService.BLL.DTO;
using CommonAsyncService.BLL.Shared;
using EmailService;
using EmailService.Shared;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;

namespace MultyHealthCheck
{
    /// <summary>
    /// Full report mode - A complete report on the system operation, generated at the request of the background process of the monitoring system
    /// healthcheck mode - Simple system health check
    /// </summary>
    public class MonitorSystems : IMonitorSystems
    {
        private readonly ILogger<MonitorSystems> _logger;
        private readonly IMapper _mapper;        
        private readonly IBllMonitorSystemsInfo _bllMonitorSystemsInfo;
        private readonly IOptions<CheckSystemsOptions> _checkSystemsOptions;
        private readonly IEmailService _emailService;

        public MonitorSystems(ILogger<MonitorSystems> logger, IMapper mapper, IBllMonitorSystemsInfo bllMonitorSystemsInfo
            ,IOptions<CheckSystemsOptions> checkSystemsOptions, IOptions<EmailSettings> emailSettings
            )
        {
            _mapper = mapper;
            _logger = logger;
            _bllMonitorSystemsInfo = bllMonitorSystemsInfo;            
            _checkSystemsOptions = checkSystemsOptions;
            _emailService = new EmailService.EmailService(emailSettings);
        }

        public async Task<CommonRezultDto> CheckAllSystems()
        {            
            var commonresult = new CommonRezultDto();
            foreach (ConsumingServicesDto consumingServiceDto in (ConsumingServicesDto[])Enum.GetValues(typeof(ConsumingServicesDto)))
            {
                if (consumingServiceDto == ConsumingServicesDto.MonitoringService)
                    continue;
                var isFullReport = await _bllMonitorSystemsInfo.NeedCheckInfo(consumingServiceDto);
                var result = await ChecSpecificSystem(isFullReport, consumingServiceDto);
                commonresult.CommonRezult.Add(consumingServiceDto, result);
            }

            var failureResult = new StringBuilder();
            foreach (var key in commonresult.CommonRezult.Keys)
            {
                if (commonresult.CommonRezult[key] == HealthCheckRezultsDto.Failure)
                {                 
                    failureResult.Append($"Error connect to {key}. ");
                }
            }

            if (failureResult.Length > 0)
            {
                var message = new EmailMessageDto
                {
                    From = "HealphCheckSystem",
                    Message = failureResult.ToString(),
                    ToEmail = "russelkov@list.ru"
                };
                await _emailService.SendMail(message);
            }
            return commonresult;
        }

        public async Task<HealthCheckRezultsDto> ChecSpecificSystem(bool isFullReport, ConsumingServicesDto consumingServicesDto)
        {
            var result = HealthCheckRezultsDto.Failure;
            switch (consumingServicesDto)
            {
                case ConsumingServicesDto.TodoService:
                    result = await CheckTodoSystem(isFullReport);
                    break;
                case ConsumingServicesDto.AuthServer:
                    result = await CheckGateAPI(isFullReport);
                    break;
            }
            return result;
        }

        public async Task<HealthCheckRezultsDto> CheckTodoSystem(bool isFullReport)
        {
            using (var channel = GrpcChannel.ForAddress(_checkSystemsOptions.Value.TodoService))
            {
                var client = new TodoClient.TodoRPC.TodoRPCClient(channel);

                try
                {
                    if (isFullReport)
                    {
                        var reply = client.SystemReport(new Google.Protobuf.WellKnownTypes.Empty());
                        var result = $"New tasks for the period {reply.TodosCount}";
                        await _bllMonitorSystemsInfo.AddServicesInfoHistory(new ServicesInfoHistoryDto
                        {
                            ConsumingService = ConsumingServicesDto.TodoService,
                            Info = result
                        });
                        return HealthCheckRezultsDto.Success;
                    }
                    else
                    {
                        var reply = client.HealthCheck(new Google.Protobuf.WellKnownTypes.Empty());
                        await _bllMonitorSystemsInfo.AddHealthCheck(new HealthsCheckDto
                        {
                            ConsumingService = ConsumingServicesDto.TodoService,
                            Rezult = HealthCheckRezultsDto.Success
                        });
                        return HealthCheckRezultsDto.Success;
                    }

                }
                catch (RpcException ex)
                {
                    await _bllMonitorSystemsInfo.AddHealthCheck(new HealthsCheckDto
                    {
                        ConsumingService = ConsumingServicesDto.TodoService,
                        Rezult = HealthCheckRezultsDto.Failure
                    });
                    return HealthCheckRezultsDto.Failure;
                }
            }
        }

        public async Task<HealthCheckRezultsDto> CheckGateAPI(bool isFullReport)
        {
            using (var httpClient = new HttpClient())
            {
                if (isFullReport)
                {
                    var response = await httpClient.GetAsync(_checkSystemsOptions.Value.AuthServer);
                    var result = response.Content.ToString();
                    await _bllMonitorSystemsInfo.AddServicesInfoHistory(new ServicesInfoHistoryDto
                    {
                        ConsumingService = ConsumingServicesDto.AuthServer,
                        Info = $"{result}"
                    });
                    return HealthCheckRezultsDto.Success;
                }
                else
                {
                    try
                    {
                        var response = await httpClient.GetAsync(_checkSystemsOptions.Value.AuthServer);
                        await _bllMonitorSystemsInfo.AddHealthCheck(new HealthsCheckDto
                        {
                            ConsumingService = ConsumingServicesDto.AuthServer,
                            Rezult = HealthCheckRezultsDto.Success
                        });
                        return HealthCheckRezultsDto.Success;
                    }
                    catch (Exception ex)
                    {
                        await _bllMonitorSystemsInfo.AddHealthCheck(new HealthsCheckDto { ConsumingService = ConsumingServicesDto.AuthServer,
                            Rezult = HealthCheckRezultsDto.Failure });
                        return HealthCheckRezultsDto.Failure;
                    }
                } 
            }
        }
    }
}
