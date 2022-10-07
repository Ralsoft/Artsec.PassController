namespace Artsec.PassController.Domain.Requests;

public class PassRequestWithPersonId : PassRequest
{
    public int PersonId => RfidPersonId != 0 ? RfidPersonId : FaceIdPersonId;  
    public int RfidPersonId { get; set; }
    public int FaceIdPersonId { get; set; }
}
