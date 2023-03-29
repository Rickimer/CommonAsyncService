using AutoMapper;
using CommonAsyncService.DtoAPI;
using MessagesQueueService.Shared;

namespace CommonAsyncService.Shared
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<EmailMessageMQ, EmailMessageDtoAPI>()
                .ReverseMap();
        }
    }
}
