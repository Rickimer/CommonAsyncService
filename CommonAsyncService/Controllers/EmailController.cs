using AutoMapper;
using CommonAsyncService.DtoAPI;
using EmailService;
using MessagesQueueService.RabbitMQ.EmailChannel;
using MessagesQueueService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CommonAsyncService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {      
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public EmailController(ILogger<EmailController> logger, IEmailService emailService, IMapper mapper)
        {
            _logger = logger;
            _emailService = emailService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Info(DateTime? forDate)
        {
            var result = 0;
            return Ok($"отправленные письма: {result}");//писем в очереди, 
        }

        //todo: добавить токен
        [HttpPost(Name = "SendEmail")]
        public async Task<ActionResult> Send(EmailMessageDtoAPI apiEmailMessage)
        {
            var emailMessage = _mapper.Map<EmailMessageMQ>(apiEmailMessage);
            var emailMqClient = new todelerabbitmqclient(); //вынести в сервис
            await emailMqClient.SendEmailAsync(emailMessage);
            return Ok();
        }
    }
}