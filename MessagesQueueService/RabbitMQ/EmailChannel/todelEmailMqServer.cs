using AutoMapper;
using EmailService;
using EmailService.Shared;
using MessagesQueueService.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessagesQueueService.RabbitMQ.EmailChannel
{
    public class todelEmailMqServer : IDisposable
    {
        IEmailService _emailService;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly ILogger<todelEmailMqServer> _logger;
        private readonly IMapper _mapper;

        //public EmailMqServer(ILogger<EmailMqServer> logger, IOptions<EmailSettings> emailSettings)
        public todelEmailMqServer(IOptions<EmailSettings> emailSettings, IMapper mapper)
        {
            //_logger = logger;
            _mapper = mapper;
            var factory = new ConnectionFactory { HostName = "localhost" };
            _emailService = new EmailService.EmailService(emailSettings);
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void StartChannelEmailServer()
        {
            {
                channel.QueueDeclare(queue: "rpc_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue",
                                 autoAck: false,
                                 consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += async (model, ea) =>
                {
                    string response = string.Empty;
                    var body = ea.Body.ToArray();

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        var emailMessageMQ = JsonConvert.DeserializeObject<EmailMessageMQ>(message);
                        if (emailMessageMQ != null)
                        {
                            var emailMessage = _mapper.Map<EmailMessageDto>(emailMessageMQ);
                            await _emailService.SendActivationMail(emailMessage);
                            _logger.LogWarning($" Sended mail to ({emailMessage.ToEmail})");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($" [.] {e.Message}");
                        response = string.Empty;
                    }
                    finally
                    {
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };
            }
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }
    }
}
