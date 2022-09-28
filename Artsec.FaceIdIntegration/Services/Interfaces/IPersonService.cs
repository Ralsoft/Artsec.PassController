namespace Artsec.PassController.Services.Interfaces;

public interface IPersonService
{
    Task<string> GetRfidPersonIdAsync(string rfid);
    Task<string> GetFaceIdPersonIdAsync(string faceId);
}
