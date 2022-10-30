namespace Artsec.PassController.Domain.Enums;

public enum AuthMode
{
    None = 0,
    RequaredRfid = 1,
    RequaredFaceId = 2,
    RequaredRfidAndFaceId = 3,
    RequaredRfidAndAnyFaceId = 4,
    AnyIdentifier = 5,
}
