using AutoMapper;
using CommonAsyncService.BLL.DTO;
using CommonAsyncService.BLL.Shared;
using CommonAsyncService.DAL.Data.Enums;
using CommonAsyncService.DAL.Data.Models;
using CommonAsyncService.DAL.Data.Repository;
using Microsoft.Extensions.Options;

namespace CommonAsyncService.BLL
{
    public class BllMonitorSystemsInfo : IBllMonitorSystemsInfo
    {
        private readonly IMapper _mapper;
        IRepository<HealthCheck> _healthChecksRepository;
        IRepository<ServicesInfoHistory> _servicesInfoHistoryRepository;
        private readonly IOptions<CheckSystemsOptions> _checkSystemsOptions;

        public BllMonitorSystemsInfo(IMapper mapper, IRepository<HealthCheck> healthChecksRepository, 
            IRepository<ServicesInfoHistory> servicesInfoHistoryRepository, IOptions<CheckSystemsOptions> checkSystemsOptions)
        {
            _mapper = mapper;
            _healthChecksRepository = healthChecksRepository;
            _servicesInfoHistoryRepository = servicesInfoHistoryRepository;
            _checkSystemsOptions = checkSystemsOptions;
        }

        public async Task<ulong> AddHealthCheck(HealthsCheckDto dto)
        {
            var entity = _mapper.Map<HealthCheck>(dto);
            entity.Created = DateTime.Now;
            await _healthChecksRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<ulong> AddServicesInfoHistory(ServicesInfoHistoryDto dto)
        {
            var entity = _mapper.Map<ServicesInfoHistory>(dto);
            entity.Created = DateTime.Now;
            await _servicesInfoHistoryRepository.AddAsync(entity);
            return entity.Id;
        }        

        public async Task<bool> NeedCheckInfo(ConsumingServicesDto consumingServicesDto)
        {
            var consumingService = _mapper.Map<ConsumingServices>(consumingServicesDto);
            var lastCheck = _servicesInfoHistoryRepository.GetQuery(e=>e.ConsumingService == consumingService).OrderByDescending(d=>d.Created)
                .FirstOrDefault()?.Created;

            if (lastCheck != null && (DateTime.Now - lastCheck.Value).Seconds >= _checkSystemsOptions.Value.MonitoringSystemsPeriodSeconds )
                return true;
            
            return false;
        }
    }
}
