using CommonAsyncService.BLL;
using CommonAsyncService.BLL.Shared;
using CommonAsyncService.DAL.Data;
using CommonAsyncService.DAL.Data.Models;
using CommonAsyncService.DAL.Data.Repository;
using CommonAsyncService.Shared;
using CommonAsyncService.Shared.Settings;
using EmailService;
using EmailService.Shared;
using HealthChecks.UI.Client;
using MessagesQueueService.RabbitMQ;
using MessagesQueueService.Shared;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using MultyHealthCheck;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEmailService, EmailService.EmailService>();

var env = builder.Environment.EnvironmentName;
if (env == "Development")
    builder.Configuration.AddUserSecrets("CommonAsync-Dev");
else
    builder.Configuration.AddUserSecrets("CommonAsync-noDev");

var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailSettings);

builder.WebHost.ConfigureLogging(
        logging =>
        {            
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        }
    ).UseNLog();

var connectionStringUsers = builder.Configuration.GetConnectionString("CommonDB");
builder.Services.AddDbContext<CommonServiceDBContext>(options =>
{
    options.UseSqlite(connectionStringUsers);
    options.ConfigureWarnings(opt =>
    {
        opt.Ignore(CoreEventId.ForeignKeyAttributesOnBothPropertiesWarning);
    });
});

builder.Services.Configure<MessageBrokerSetings>(builder.Configuration.GetSection("MessageBrokerSetings"));
builder.Services.Configure<CheckSystemsOptions>(builder.Configuration.GetSection("HealthCheckOptions"));

builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddAutoMapper(typeof(BllMappingProfile));
builder.Services.AddAutoMapper(typeof(MessagesQueueMappingProfile));

builder.Services.AddScoped(typeof(IRepository<HealthCheck>), typeof(HealthChecksRepository));
builder.Services.AddScoped(typeof(IRepository<ServicesInfoHistory>), typeof(ServicesInfoHistoryRepository));

builder.Services.AddScoped(typeof(IRepository<StoryMailProcessing>), typeof(StoryMailProcessingRepository));
builder.Services.AddScoped(typeof(IRepository<User>), typeof(UserRepository));

builder.Services.AddScoped<IMonitorSystems, MonitorSystems>();
builder.Services.AddScoped<IBllMonitorSystemsInfo, BllMonitorSystemsInfo>();

builder.Services.AddControllers();
builder.Services.AddHostedService<EmailWorker>();

builder.Services
    .AddHealthChecks()    
    .AddCheck<RequestHealthCheck>("RequestTimeCheck")    
    ;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var f = emailSettings.Get<EmailSettings>();
IOptions<EmailSettings> d =  Options.Create(f);

var app = builder.Build();
//app.MapHealthChecks("/health");
//app.MapGrpcService<TodoRPCService>();

app.UseHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
