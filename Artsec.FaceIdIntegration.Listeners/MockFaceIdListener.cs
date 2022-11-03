using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Artsec.PassController.Listeners;

public class MockFaceIdListener
{
    private readonly ILogger<MockFaceIdListener> _logger;
    private readonly IOptions<FaceIdListenerConfiguration> _options;
    private bool _isReceiving;

    public event EventHandler<ReceivedFaceIdEventArgs> DataReceived;
    public string SourceName => "FaceId";
    public string SourceType => "HTTP";

    public MockFaceIdListener(ILogger<MockFaceIdListener> logger, IOptions<FaceIdListenerConfiguration> options)
    {
        _logger = logger;
        _options = options;

    }
    private async Task<string> GenerateMessage()
    {
        var rand = new Random();
        var delay = TimeSpan.FromSeconds(rand.Next(1, 60));
        await Task.Delay(delay);
        return "";
    }
    public async Task ReceiveMessage()
    {
        string url = _options.Value.Url;
        var regex = new Regex(@"\{(.|\s)*\}");

        while (_isReceiving)
        {
            try
            {
                bool isReaded = false;
                var message = await GenerateMessage();
                //_logger?.LogDebug(message);

                var match = regex.Match(message);
                if (match.Success)
                {
                    isReaded = true;
                    var faceIdMessage = JsonSerializer.Deserialize<FaceIdMessage>(match.Value);
                    if (faceIdMessage.CamId != string.Empty)
                    {
                        MessageReceived?.Invoke(this, new ReceivedFaceIdEventArgs() { Message = faceIdMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error: {ex.Message}", ex);
                await Task.Delay(5000);
            }
        }
    }

    public event EventHandler<ReceivedFaceIdEventArgs> MessageReceived;


    public void StartListen()
    {
        _isReceiving = true;
        _ = ReceiveMessage();
    }

    public void StopListen()
    {
        _isReceiving = false;
    }
}
