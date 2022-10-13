using Artsec.PassController.Domain;

namespace Artsec.PassController.Services.Interfaces;

public interface ICommandSender
{
    Task SendAllowPassAsync(byte[] data, string? remoteAddress, int remotePort);
    Task SendRejectPassAsync(byte[] data, string? remoteAddress, int remotePort);
    Task SendCommandAsync(ControllerCommand command, string? remoteAddress, int remotePort);
    Task SendCommandAsync(byte[] data, string? remoteAddress, int remotePort);
    Task SendOpenDoorAsync(int channel, string? remoteAddress, int remotePort);
}
