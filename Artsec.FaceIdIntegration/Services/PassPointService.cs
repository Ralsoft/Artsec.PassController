using Artsec.PassController.Configs;
using Artsec.PassController.Domain;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Artsec.PassController.Services;

public class PassPointService : IPassPointService
{
    private readonly IOptions<ControllersConfigurations> _options;

    public PassPointService(IOptions<ControllersConfigurations> options)
    {
        _options = options;
    }
    public int GetPassPointId(string ip, int channelNumber)
    {
        return _options.Value.Controllers[ip].Channels[channelNumber.ToString()];
    }

    public int GetPassPointIdForFaceId(int cameraId)
    {
        return _options.Value.CamIdToDevId[cameraId.ToString()];
    }
    public Controller GetControllerByDeviceId(int deviceId)
    {
        foreach (var (ip, controller) in _options.Value.Controllers)
        {
            if (controller.Channels.ContainsValue(deviceId))
                return controller;
        }
        throw new ControllerNotFoundException($"Channel {deviceId} not found in configs");
    }
}
