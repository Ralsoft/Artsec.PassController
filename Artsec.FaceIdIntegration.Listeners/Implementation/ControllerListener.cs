using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Events;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Artsec.PassController.Listeners.Implementation;

public class ControllerListener
{
    private readonly ILogger<ControllerListener> _logger;
    private readonly UdpClient _udpClient;
    private readonly ControllerListenerConfiguration _config;
    private readonly int _port;
    private bool _isReceiving;

    public ControllerListener(ILogger<ControllerListener> logger, ControllerListenerConfiguration config)
    {
        _logger = logger;
        _config = config;

        _udpClient = new UdpClient(_config.Port);
        _port = ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port;
        DataReceived = OnDataReceived;

        Task.Run(ReceiveMessage);
    }

    public event EventHandler<ReceivedRfidEventArgs> DataReceived;

    public string SourceName => "Contoller";
    public string SourceType => "UDP";
    public string Ip => _config.Ip;
    public int Port => _port;


    public void StartListen()
    {
        _isReceiving = true;
    }

    public void StopListen()
    {
        _isReceiving = false;
    }

    private void OnDataReceived(object? sender, ReceivedRfidEventArgs e)
    {
        _logger?.LogInformation(string.Join(" ", e.RemoteIp));
        _logger?.LogInformation(string.Join(" ", e.Data));

        // TODO: 
    }


    public async Task ReceiveMessage()
    {
        try
        {
            _logger?.LogInformation($"Start receiving messages on port: {_port}");
            Console.WriteLine($"Start receiving messages on port: {_port}");
            while (true)
            {
                if (_isReceiving)
                {
                    var result = await _udpClient.ReceiveAsync(); // получаем данные
                    byte[] data = result.Buffer;
                    string message = BitConverter.ToString(data);
                    _logger?.LogInformation($"Receiver get message: {message} from {result.RemoteEndPoint}");
                    DataReceived?.Invoke(this, new ReceivedRfidEventArgs(data, result.RemoteEndPoint));
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message);
        }
        finally
        {
            _ = Task.Run(ReceiveMessage);
        }
    }
}