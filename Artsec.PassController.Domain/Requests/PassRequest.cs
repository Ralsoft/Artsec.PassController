using Artsec.PassController.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Domain.Requests;

public class PassRequest
{
    public int DeviceId { get; set; }
    public string? CamId { get; set; }
    public string? Rfid { get; set; }
    public string? FaceId { get; set; }
    public AuthMode AuthMode { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public int Channel { get; set; }
    public string? RemoteAddress { get; set; }
    public int RemotePort { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public bool IsReadyToProcessing()
    {
        if (DateTime.Now - CreationTime > TimeSpan.FromSeconds(5))
            return true;


        switch (AuthMode)
        {
            case AuthMode.None:
                return true;
            case AuthMode.RequaredRfid:
                return Rfid != null;
            case AuthMode.RequaredRfidAndFaceId:
                return Rfid != null && FaceId != null;
            case AuthMode.RequaredRfidAndAnyFaceId:
                return Rfid != null && FaceId != null;
            case AuthMode.RequaredFaceId:
                return FaceId != null;
            case AuthMode.AnyIdentifier:
                return Rfid != null || FaceId != null;
            default:
                return false;
        }
    }
}
