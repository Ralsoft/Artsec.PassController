using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Events;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Artsec.PassController.Listeners.Implementation;

public class FaceIdListener
{
    private readonly ILogger<FaceIdListener> _logger;
    private readonly FaceIdListenerConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private bool _isReceiving;

    public event EventHandler<ReceivedFaceIdEventArgs> DataReceived;
    public string SourceName => "FaceId";
    public string SourceType => "HTTP";

    public FaceIdListener(ILogger<FaceIdListener> logger, FaceIdListenerConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config;
        _httpClientFactory = httpClientFactory;
        _httpClient = new HttpClient();

    }
    public async Task ReceiveMessage()
    {
        string url = _config.Url;
        var regex = new Regex(@"\{(.|\s)*\}");
        var httpClient = _httpClientFactory.CreateClient();

        while (_isReceiving)
        {
            try
            {
                using var streamReader = new StreamReader(await httpClient.GetStreamAsync(url));
                bool isReaded = false;
                while (!streamReader.EndOfStream && !isReaded)
                {
                    var message = await streamReader.ReadLineAsync();
                    _logger?.LogDebug(message);

                    var match = regex.Match(message);
                    if (match.Success)
                    {
                        _logger?.LogDebug("Matched!");
                        isReaded = true;
                        var faceIdMessage = JsonSerializer.Deserialize<FaceIdMessage>(match.Value);
                        if(faceIdMessage.CamId != string.Empty)
                        {
                            MessageReceived?.Invoke(this, new ReceivedFaceIdEventArgs() { Message = faceIdMessage });
                        }
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
        _ = ReceiveMessage();
    }

    public void StopListen()
    {
        _isReceiving = false;
    }
}
