namespace Artsec.PassController.Domain.Requests;

public class PassRequestWithPersonId : PassRequest
{
    public PassRequestWithPersonId(PassRequest request)
    {
        DeviceId = request.DeviceId;
        Rfid = request.Rfid;
        FaceId = request.FaceId;
        AuthMode = request.AuthMode;
        CreationTime = request.CreationTime;
        Channel = request.Channel;
        RemoteAddress = request.RemoteAddress;
        RemotePort = request.RemotePort;
        Data = request.Data;
        CamId = request.CamId;
    }
    public int PersonId => RfidPersonId != 0 ? RfidPersonId : FaceIdPersonId;  
    public int RfidPersonId { get; set; }
    public int FaceIdPersonId { get; set; }
}
