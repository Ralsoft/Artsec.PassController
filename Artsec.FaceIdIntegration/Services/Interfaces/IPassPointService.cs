namespace Artsec.PassController.Services.Interfaces;

public interface IPassPointService
{
    int GetPassPointId(string ip, int channelNumber);
    int GetPassPointIdForFaceId(int cameraId);
}
