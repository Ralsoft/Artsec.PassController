using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Artsec.PassController.Listeners;

public class FaceIdListener
{
    private readonly ILogger<FaceIdListener> _logger;
    private readonly IOptions<FaceIdListenerConfiguration> _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private bool _isReceiving;

    public event EventHandler<ReceivedFaceIdEventArgs> DataReceived;
    public string SourceName => "FaceId";
    public string SourceType => "HTTP";

    public FaceIdListener(ILogger<FaceIdListener> logger, IOptions<FaceIdListenerConfiguration> options, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _options = options;
        _httpClientFactory = httpClientFactory;
        _httpClient = new HttpClient();

    }
    public async Task ReceiveMessage()
    {
        string url = _options.Value.Url;
        var regex = new Regex(@"\{(.|\s)*\}");

        while (_isReceiving)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                using var streamReader = new StreamReader(await httpClient.GetStreamAsync(url));
                bool isRead = false;
                while (!streamReader.EndOfStream && !isRead)
                {
                    var message = await streamReader.ReadLineAsync() ?? string.Empty;
                    //_logger?.LogDebug(message);

                    var match = regex.Match(message);
                    if (match.Success)
                    {
                        isRead = true;
                        var faceIdMessage = JsonSerializer.Deserialize<FaceIdMessage>(match.Value);
                        if (faceIdMessage is not null && faceIdMessage.CamId != string.Empty)
                        {
                            MessageReceived?.Invoke(this, new ReceivedFaceIdEventArgs() { Message = faceIdMessage });
                        }
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
