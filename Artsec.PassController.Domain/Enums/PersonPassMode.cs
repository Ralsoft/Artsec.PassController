namespace Artsec.PassController.Domain.Enums;

public enum PersonPassMode
{
    None = 0,
    RequaredRfid = 1,
    RequaredRfidAndPersonFaceId = 2,
    RequaredRfidAndAnyFaceId = 3,
    RequaredFaceId = 4,
    AnyIdentifier = 5,
}
