﻿using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Pipelines.Middleware;
using Artsec.PassController.Services.Interfaces;
using PipeLight;
using PipeLight.Pipes;
using PipeLight.Extensions;

namespace Artsec.PassController.Pipelines;

public class PassRequestPipeline : Pipe<PassRequest>
{
	private readonly ILogger<PassRequestPipeline> _logger;
	public PassRequestPipeline(IServiceProvider serviceProvider, ILogger<PassRequestPipeline> logger)
    {
        _logger = logger;
        this
        .AddStep(new ValidationStep(serviceProvider.GetRequiredService<IValidationService>(),
                                    serviceProvider.GetRequiredService<ILogger<ValidationStep>>()))
        .AddStep(x => { _logger?.LogInformation($"Получен код валидации: {x.ValidCode}"); return x; })
        .AddStep(new CommandSenderStep(serviceProvider.GetRequiredService<ICommandSender>(),
                                                   serviceProvider.GetRequiredService<ILogger<CommandSenderStep>>()))
        .AddStep(r => { _logger?.LogInformation($"Время от получения запроса до открытия двери: {DateTime.Now - r.CreationTime}"); return r; })
        .AddStep(new RequestsLoggingStep(serviceProvider.GetRequiredService<IRequestsLoggingService>()))
        .AddStep(new MqttStep(serviceProvider.GetRequiredService<IMqttService>()))
        .AddStep(r => { _logger?.LogInformation($"Время полной обработки запроса: {DateTime.Now - r.CreationTime}"); return r; })
        ;
    }
}
