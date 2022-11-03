using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Nodes.Steps.Interfaces;

namespace Artsec.PassController.Pipelines.Middleware;

internal class CommandSenderStep : IPipeStep<PassRequest>
{
    private readonly ICommandSender _commandSender;
    private readonly ILogger<CommandSenderStep> _logger;

    public CommandSenderStep(ICommandSender commandSender, ILogger<CommandSenderStep> logger)
    {
        _commandSender = commandSender;
        _logger = logger;
    }


    public async Task<PassRequest> ExecuteStepAsync(PassRequest payload)
    {
        try
        {
            if (payload.IsValid)
            {
                switch (payload.AuthMode)
                {
                    case AuthMode.None:
                        _logger?.LogWarning($"Не указан тип авторизации. Команда не отправлена");
                        break;
                    case AuthMode.RequaredRfid:
                        await _commandSender.SendAllowPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
                        _logger?.LogInformation($"Для RFID = {payload.Rfid} отправлена команда на разрешение прохода");
                        break;
                    case AuthMode.RequaredRfidAndFaceId:
                        await _commandSender.SendAllowPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
                        _logger?.LogInformation($"Для RFID = {payload.Rfid} отправлена команда на разрешение прохода");
                        break;
                    case AuthMode.RequaredRfidAndAnyFaceId:
                        await _commandSender.SendAllowPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
                        _logger?.LogInformation($"Для RFID = {payload.Rfid} отправлена команда на разрешение прохода");
                        break;
                    case AuthMode.RequaredFaceId:
                        await _commandSender.SendOpenDoorAsync(payload.Channel, payload.RemoteAddress, payload.RemotePort);
                        _logger?.LogInformation($"Для FaceId = {payload.FaceId} отправлена команда на открытие двери");
                        break;
                    case AuthMode.AnyIdentifier:
                        if (payload.Rfid is not null)
                        {
                            await _commandSender.SendAllowPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
                            _logger?.LogInformation($"Для RFID = {payload.Rfid} отправлена команда на разрешение прохода");
                        }
                        else
                        {
                            await _commandSender.SendOpenDoorAsync(payload.Channel, payload.RemoteAddress, payload.RemotePort);
                            _logger?.LogInformation($"Для FaceId = {payload.FaceId} отправлена команда на открытие двери");
                        }
                        break;
                    default:
                        _logger?.LogWarning($"Неизвестный тип авторизации. Команда не отправлена");
                        break;
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
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex.Message);
            _logger?.LogCritical(ex.StackTrace);
        }
        return payload;
    }
}
