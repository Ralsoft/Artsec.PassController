using Artsec.PassController.Listeners.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

    public ControllerListener(ILogger<ControllerListener> logger, ControllerListenerConfiguration config) : this(config)
    {
        _logger = logger;
    }
    public ControllerListener(ControllerListenerConfiguration config)
    {
        _config = config;
        _udpClient = new UdpClient(_config.Port);
        _port = ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port;
        DataReceived = OnDataReceived;

        Task.Run(ReceiveMessage);
    }

    public event EventHandler<ReceivedDataEventArgs> DataReceived;

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

    private void OnDataReceived(object? sender, ReceivedDataEventArgs e)
    {
        Console.WriteLine(e.RemoteIp);
        Console.WriteLine(e.Data);

        // TODO: 
    }


    public void ReceiveMessage()
    {
        IPEndPoint? remoteIp = null; // адрес входящего подключения
        try
        {
            _logger?.LogInformation($"Start receiving messages on port: {_port}");
            Console.WriteLine($"Start receiving messages on port: {_port}");
            while (true)
            {
                if (_isReceiving)
                {
                    byte[] data = _udpClient.Receive(ref remoteIp); // получаем данные
                    string message = BitConverter.ToString(data);
                    _logger?.LogInformation($"Receiver get message: {message} from {remoteIp}");
                    Console.WriteLine($"Receiver get message: {message} from {remoteIp}");
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    DataReceived?.Invoke(this, new ReceivedDataEventArgs(data, remoteIp));
                    sw.Stop();
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message);
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Task.Run(ReceiveMessage);
        }
    }
}

public class ReceivedDataEventArgs : EventArgs
{
    public ReceivedDataEventArgs(byte[] data, IPEndPoint? remoteIp)
    {
        Data = data;
        RemoteIp = remoteIp;
    }
    public byte[] Data { get; }
    public IPEndPoint? RemoteIp { get; }
}
