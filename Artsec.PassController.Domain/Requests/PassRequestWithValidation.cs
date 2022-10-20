namespace Artsec.PassController.Domain.Requests;

public class PassRequestWithValidation : PassRequestWithPersonId
{
    public PassRequestWithValidation(PassRequestWithPersonId request) : base(request)
    {
        RfidPersonId = request.RfidPersonId;
        FaceIdPersonId = request.FaceIdPersonId;
    }
    public bool IsValid { get; set; }
    public int ValidCode { get; set; }
}
