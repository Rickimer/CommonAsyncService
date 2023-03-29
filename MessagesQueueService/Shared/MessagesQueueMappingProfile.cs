using AutoMapper;
using EmailService.Shared;

namespace MessagesQueueService.Shared
{
    public class MessagesQueueMappingProfile : Profile
    {
        public MessagesQueueMappingProfile()
        {
            CreateMap<EmailMessageMQ, EmailMessageDto>()
                    .ReverseMap();
        }
    }
}
