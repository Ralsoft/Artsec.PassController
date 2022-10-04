using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Events;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Artsec.PassController.Listeners.Implementation;

public class FaceIdListener
{
    private readonly ILogger<FaceIdListener> _logger;
    private readonly FaceIdListenerConfiguration _config;
    private readonly HttpClient _httpClient;
    private bool _isReceiving;

    public event EventHandler<ReceivedFaceIdEventArgs> DataReceived;
    public string SourceName => "FaceId";
    public string SourceType => "HTTP";

    public FaceIdListener(ILogger<FaceIdListener> logger, FaceIdListenerConfiguration config, HttpClient httpClient)
    {
        _logger = logger;
        _config = config;
        _httpClient = httpClient;

        Task.Run(ReceiveMessage);
    }
    public async Task ReceiveMessage()
    {
        string url = _config.Url;

        while (true)
        {
            try
            {
                if (_isReceiving)
                {
                    using var streamReader = new StreamReader(await _httpClient.GetStreamAsync(url));
                    var sb = new StringBuilder();
                    while (!streamReader.EndOfStream)
                    {
                        var message = await streamReader.ReadLineAsync();
                        _logger?.LogInformation(message);

                        var faceIdMessage = JsonSerializer.Deserialize<FaceIdMessage>(message);
                        MessageReceived?.Invoke(this, new ReceivedFaceIdEventArgs(faceIdMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error: {ex.Message}");
            }
        }
    }

    public event EventHandler<ReceivedFaceIdEventArgs> MessageReceived;


    public void StartListen()
    {
        _isReceiving = true;
    }

    public void StopListen()
    {
        _isReceiving = false;
    }
}
