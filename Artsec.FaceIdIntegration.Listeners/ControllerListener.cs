using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace Artsec.PassController.Listeners;

public class ControllerListener
{
    private readonly ILogger<ControllerListener> _logger;
    private readonly UdpClient _udpClient;
    private readonly IOptions<ControllerListenerConfiguration> _options;
    private readonly int _port;
    private bool _isReceiving;

    public ControllerListener(ILogger<ControllerListener> logger, IOptions<ControllerListenerConfiguration> options)
    {
        _logger = logger;
        _options = options;

        _udpClient = new UdpClient(_options.Value.Port);
        _port = ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port;
    }

    public event EventHandler<ReceivedRfidEventArgs> MessageReceived;

    public string SourceName => "Contoller";
    public string SourceType => "UDP";
    public string Ip => _options.Value.Ip;
    public int Port => _port;


    public void StartListen()
    {
        _isReceiving = true;
        _ = ReceiveMessage();
    }

    public void StopListen()
    {
        _isReceiving = false;
    }

    public async Task ReceiveMessage()
    {
        while (_isReceiving)
        {
            try
            {
                _logger?.LogInformation($"Start receiving messages on port: {_port}");
                var result = await _udpClient.ReceiveAsync(); // получаем данные
                byte[] data = result.Buffer;
                string message = BitConverter.ToString(data);
                _logger?.LogInformation($"Receiver get message: {message} from {result.RemoteEndPoint}");
                MessageReceived?.Invoke(this, new ReceivedRfidEventArgs(data, result.RemoteEndPoint, null));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message, ex);
            }
        }
    }
}