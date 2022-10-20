using Artsec.PassController.Configs;
using Artsec.PassController.Domain;
using Artsec.PassController.Domain.Exceptions;
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
    public Controller GetControllerByDeviceId(int deviceId)
    {
        foreach (var (ip, controller) in _configs.Controllers)
        {
            if (controller.Channels.ContainsValue(deviceId))
                return controller;
        }
        throw new ControllerNotFoundException($"Channel {deviceId} not found in configs");
    }
}
