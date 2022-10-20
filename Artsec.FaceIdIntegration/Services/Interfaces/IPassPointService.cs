using Artsec.PassController.Domain;

namespace Artsec.PassController.Services.Interfaces;

public interface IPassPointService
{
    Controller? GetControllerByDeviceId(int deviceId);
    int GetPassPointId(string ip, int channelNumber);
    int GetPassPointIdForFaceId(int cameraId);
}
