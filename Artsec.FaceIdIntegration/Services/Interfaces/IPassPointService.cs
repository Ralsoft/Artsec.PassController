namespace Artsec.PassController.Services.Interfaces;

public interface IPassPointService
{
    string GetPassPointId(string ip);
    string GetPassPointIdForFaceId(int cameraId);
}
