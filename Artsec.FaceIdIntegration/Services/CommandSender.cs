using Artsec.PassController.Domain;
using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Services.Interfaces;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Artsec.PassController.Services;

internal class CommandSender : ICommandSender
{
    private readonly ILogger<CommandSender> _logger;
    public CommandSender(ILogger<CommandSender> logger)
    {
        _logger = logger;
    }

    public async Task SendAllowPassAsync(byte[] data, string? remoteAddress, int remotePort)
    {
        var message = RfidMessage.Parse(data);
        var packager = new ArtonitSePackager(ArtonitSeCommand.Answer);
        packager.AddArgs(message.Body.Take(4).ToArray());
        packager.AddArg(message.Channel);
        packager.AddArg(0x03);
        var sendMessage = packager.Pack();
        await SendCommandAsync(sendMessage, remoteAddress, remotePort);
    }
    public async Task SendRejectPassAsync(byte[] data, string? remoteAddress, int remotePort)
    {
        var message = RfidMessage.Parse(data);
        var packager = new ArtonitSePackager(ArtonitSeCommand.Answer);
        packager.AddArgs(message.Body.Take(4).ToArray());
        packager.AddArg(message.Channel);
        packager.AddArg(0x01);
        var sendMessage = packager.Pack();
        await SendCommandAsync(sendMessage, remoteAddress, remotePort);
    }

    public async Task SendCommandAsync(ControllerCommand command, string? remoteAddress, int remotePort)
    {
        await SendCommandAsync(command.Data, remoteAddress, remotePort);
    }
    public async Task SendCommandAsync(byte[] data, string? remoteAddress, int remotePort)
    {
        try
        {
            using var udpClient = new UdpClient();
            await udpClient.SendAsync(data, data.Length, remoteAddress, remotePort);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task SendOpenDoorAsync(int channel, string? remoteAddress, int remotePort)
    {
        byte[] data;
        if (channel == 0)
            data = new byte[]
            {
                0x02,
                0x1F,
                0x00,
                0x03,
                0x61,
                0x38,
            };
        else
            data = new byte[]
            {
                0x02,
                0x1F,
                0x01,
                0x03,
                0xB9,
                0x21,
            };
        await SendCommandAsync(data, remoteAddress, remotePort);
    }
}