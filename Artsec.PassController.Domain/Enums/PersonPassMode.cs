namespace Artsec.PassController.Domain.Enums;

public enum AuthMode
{
    None = 0,
    RequaredRfid = 1,
    RequaredRfidAndFaceId = 2,
    RequaredRfidAndAnyFaceId = 3,
    RequaredFaceId = 4,
    AnyIdentifier = 5,
}
