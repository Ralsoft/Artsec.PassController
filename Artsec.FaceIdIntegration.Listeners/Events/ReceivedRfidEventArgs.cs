using System.Net;

namespace Artsec.PassController.Listeners.Events;

public class ReceivedRfidEventArgs 
{
    public ReceivedRfidEventArgs(byte[] data, IPEndPoint remoteIp)
    {
        Data = data;
        RemoteIp = remoteIp;
    }
    public byte[] Data { get; }
    public IPEndPoint RemoteIp { get; }
    public string Rfid { get; set; } = string.Empty;
}