using Artsec.PassController.Configs;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

public class PassPointService : IPassPointService
{
    private readonly WorkerConfigurations _configs;

    public PassPointService(WorkerConfigurations configs)
    {
        _configs = configs;
    }
    public int GetPassPointId(string ip, int channelNumber)
    {
        return _configs.Controllers[ip].Channels[channelNumber.ToString()];
    }

    public int GetPassPointIdForFaceId(int cameraId)
    {
        return _configs.CamIdToDevId[cameraId.ToString()];
    }
}
