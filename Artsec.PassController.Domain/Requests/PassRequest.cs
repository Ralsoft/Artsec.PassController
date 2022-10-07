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
    public string? Rfid { get; set; }
    public string? FaceId { get; set; }
    public AuthMode AuthMode { get; set; }
    public DateTime CreationTime { get; } = DateTime.Now;
    public int Channel { get; set; }
    public string? RemoteAddress { get; set; }
    public int RemotePort { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public bool IsReadyToProcessing()
    {
        if (DateTime.Now - CreationTime > TimeSpan.FromSeconds(5))
            return true;


        Func<bool> validate = () => false;
        if (AuthMode == AuthMode.RequaredRfid)
            validate = () => Rfid != null;
        else if (AuthMode == AuthMode.RequaredRfidAndFaceId)
            validate = () => Rfid != null && FaceId != null;
        else if (AuthMode == AuthMode.RequaredRfidAndAnyFaceId)
            validate = () => Rfid != null && FaceId != null;
        else if (AuthMode == AuthMode.RequaredFaceId)
            validate = () => FaceId != null;
        else if (AuthMode == AuthMode.AnyIdentifier)
            validate = () => Rfid != null || FaceId != null;

        return validate.Invoke();
    }
}
