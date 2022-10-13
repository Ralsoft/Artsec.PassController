using System.Net;

namespace Artsec.PassController.Listeners.Events;

public class ReceivedRfidEventArgs 
{
    public ReceivedRfidEventArgs(byte[] data, IPEndPoint remoteIp, string? rfid)
    {
        Data = data;
        RemoteIp = remoteIp;
        Rfid = rfid;
    }
    public byte[] Data { get; }
    public IPEndPoint RemoteIp { get; }
    public string? Rfid { get; set; }
}