using AutoMapper;
using CommonAsyncService.BLL.DTO;
using CommonAsyncService.DAL.Data.Enums;
using CommonAsyncService.DAL.Data.Models;

namespace CommonAsyncService.BLL.Shared
{
    public class BllMappingProfile : Profile
    {
        public BllMappingProfile()
        {
            CreateMap<ConsumingServices, ConsumingServicesDto>()
                    .ReverseMap();

            CreateMap<HealthCheck, HealthsCheckDto>()
                    .ReverseMap();
        }
    }
}
