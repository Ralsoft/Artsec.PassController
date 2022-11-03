using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Nodes.Steps.Interfaces;
using System.Text.Json;

namespace Artsec.PassController.Pipelines.Middleware;

internal class MqttStep : IPipeStep<PassRequest>
{
    private readonly IMqttService _mqttService;

    public MqttStep(IMqttService mqttService)
    {
        _mqttService = mqttService;
    }

    public async Task<PassRequest> ExecuteStepAsync(PassRequest payload)
    {
        var message = JsonSerializer.Serialize(payload);
        await _mqttService.SendMessageAsync(message);
        return payload;
    }
}
