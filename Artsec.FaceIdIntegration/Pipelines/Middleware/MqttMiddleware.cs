using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Middlewares.Interfaces;
using System.Text.Json;

namespace Artsec.PassController.Pipelines.Middleware;

internal class MqttMiddleware : IPipelineMiddleware<PassRequestWithValidation, PassRequestWithValidation>
{
    private readonly IMqttService _mqttService;

    public MqttMiddleware(IMqttService mqttService)
    {
        _mqttService = mqttService;
    }
    public async Task<PassRequestWithValidation> InvokeAsync(PassRequestWithValidation payload)
    {
        var message = JsonSerializer.Serialize(payload);
        await _mqttService.SendMessageAsync(message);
        return payload;
    }
}
