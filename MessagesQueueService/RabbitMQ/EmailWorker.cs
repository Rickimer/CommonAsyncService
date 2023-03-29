using AutoMapper;
using EmailService;
using EmailService.Shared;
using MessagesQueueService.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace MessagesQueueService.RabbitMQ
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;        
        private const string QueueName = "emailworker";
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private IModel _channel;        

        public EmailWorker(ILogger<EmailWorker> logger, IOptions<EmailSettings> emailSettings, IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _emailService = new EmailService.EmailService(emailSettings);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory
            {
                /*UserName = "ops1",
                Password = "ops1",*/
                HostName = "localhost",
                ConsumerDispatchConcurrency = 2,
                DispatchConsumersAsync = true
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            //_channel.QueueDeclarePassive(QueueName);
            _channel.QueueDeclare(queue: QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (bc, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation($"Processing msg: '{message}'.");
                try
                {
                    var emailMessageMQ = JsonConvert.DeserializeObject<EmailMessageMQ>(message);
                    if (emailMessageMQ != null)
                    {
                        _logger.LogInformation($"Sending email to [{emailMessageMQ.ToEmail}].");

                        if (emailMessageMQ != null)
                        {                            
                            var emailMessage = _mapper.Map<EmailMessageDto>(emailMessageMQ);
                            switch (emailMessage.emailType)
                            {
                                case EmailTypeDto.Common:
                                    await _emailService.SendMail(emailMessage);
                                    break;
                                case EmailTypeDto.ActivationEmail:
                                    await _emailService.SendActivationMail(emailMessage);
                                    break;
                            }
                            
                            _logger.LogWarning($" Sended mail to ({emailMessage.ToEmail})");
                        }                        
                    }
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    _logger.LogError($"JSON Parse Error: '{message}'.");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                }
                catch (Exception e)
                {
                    _logger.LogError(default, e, e.Message);
                }
                finally
                {
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection is closed.");
        }               
    }
}
