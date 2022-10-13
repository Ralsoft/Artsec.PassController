using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Middlewares.Interfaces;

namespace Artsec.PassController.Pipelines.Middleware;

internal class CommandSenderMiddleware : IPipelineMiddleware<PassRequestWithValidation, PassRequestWithValidation>
{
    private readonly ICommandSender _commandSender;
    private readonly ILogger<CommandSenderMiddleware> _logger;

    public CommandSenderMiddleware(ICommandSender commandSender, ILogger<CommandSenderMiddleware> logger)
    {
        _commandSender = commandSender;
        _logger = logger;
    }

    public async Task<PassRequestWithValidation> InvokeAsync(PassRequestWithValidation payload)
    {
        if (payload.IsValid)
        {
            if (payload.AuthMode == AuthMode.RequaredFaceId ||
               (payload.AuthMode == AuthMode.AnyIdentifier && payload.FaceId != null))
            {
                await _commandSender.SendOpenDoorAsync(payload.Channel, payload.RemoteAddress, payload.RemotePort);
                _logger?.LogInformation($"Для FaceId = {payload.FaceId} отправлена команда на открытие двери");
            }
            else
            {
                await _commandSender.SendAllowPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
                _logger?.LogInformation($"Для RFID = {payload.Rfid} отправлена команда на разрешение прохода");
            }
        }
        else
        {
            if (payload.Rfid is not null)
            {
                await _commandSender.SendRejectPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
                _logger?.LogInformation($"Валидация не прошла. Для RFID = {payload.Rfid} отправлена команда \"Запрет прохода\"");
            }
            else
            {
                _logger?.LogInformation($"Валидация не прошла. Команда не отправлена");
            }
        }
        return payload;
    }
}
