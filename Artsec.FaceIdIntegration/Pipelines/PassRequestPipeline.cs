using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Pipelines.Middleware;
using Artsec.PassController.Services.Interfaces;
using PipeLight;

namespace Artsec.PassController.Pipelines;

public class PassRequestPipeline : Pipeline<PassRequestWithPersonId>
{
	private readonly ILogger<PassRequestPipeline> _logger;
	public PassRequestPipeline(IServiceProvider serviceProvider, ILogger<PassRequestPipeline> logger)
    {
        _logger = logger;
        this
        .AddMiddleware(new ValidationMiddleware(serviceProvider.GetRequiredService<IValidationService>(),
                                                   serviceProvider.GetRequiredService<ILogger<ValidationMiddleware>>()))
        .AddFunc(x => { _logger?.LogInformation($"Получен код валидации: {x.ValidCode}"); return x; })
        .AddMiddleware(new CommandSenderMiddleware(serviceProvider.GetRequiredService<ICommandSender>(),
                                                   serviceProvider.GetRequiredService<ILogger<CommandSenderMiddleware>>()))
        .AddFunc(r => { _logger?.LogInformation($"Время от получения запроса до открытия двери: {DateTime.Now - r.CreationTime}"); return r; })
        .AddMiddleware(new RequestsLoggingMiddleware(serviceProvider.GetRequiredService<IRequestsLoggingService>()))
        .AddMiddleware(new MqttMiddleware(serviceProvider.GetRequiredService<IMqttService>()))
        .AddFunc(r => { _logger?.LogInformation($"Время полной обработки запроса: {DateTime.Now - r.CreationTime}"); return r; })
        ;
    }
}
