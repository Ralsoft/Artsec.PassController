using Artsec.PassController.Configs;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace Artsec.PassController.Services;

public class MqttService : IMqttService
{
    private readonly MqttFactory _mqttFactory;
    private readonly ILogger<MqttService> _logger;
    private readonly IOptions<MqttConfigurations> _options;

    public MqttService(ILogger<MqttService> logger, IOptions<MqttConfigurations> options, MqttFactory mqttFactory)
    {
        _logger = logger;
        _options = options;
        _mqttFactory = mqttFactory;
    }
    public async Task SendMessageAsync(string message)
    {
        try
        {
            _logger?.LogInformation($"Отправка сообщения {message} в топик {_options.Value.Topic}");
            using var mqttClient = _mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_options.Value.MqttServer)
                .WithClientId(_options.Value.MqttClientId)
                .Build();
            await mqttClient.ConnectAsync(mqttClientOptions);

            var mqttMessage = _mqttFactory.CreateApplicationMessageBuilder()
                .WithTopic(_options.Value.Topic)
                .WithPayload(message)
                .Build();
            await mqttClient.PublishAsync(mqttMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError("Не удалось отправить сообщение на MQTT сервер\n" + ex.Message, ex);
        }
    }
}
