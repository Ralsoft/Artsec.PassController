using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Middlewares.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Pipelines.Middleware;

internal class CommandSenderMiddleware : IPipelineMiddleware<PassRequestWithValidation, PassRequestWithValidation>
{
    private readonly ICommandSender _commandSender;

    public CommandSenderMiddleware(ICommandSender commandSender)
    {
        _commandSender = commandSender;
    }

    public async Task<PassRequestWithValidation> InvokeAsync(PassRequestWithValidation payload)
    {
        if (payload.IsValid)
        {
            if (payload.AuthMode == Domain.Enums.AuthMode.RequaredFaceId ||
                (payload.AuthMode == Domain.Enums.AuthMode.AnyIdentifier && payload.FaceId != null))
            {
                await _commandSender.SendOpenDoorAsync(payload.Channel, payload.RemoteAddress, payload.RemotePort);
            }
            else
            {
                await _commandSender.SendAllowPassAsync(payload.Data, payload.RemoteAddress, payload.RemotePort);
            }
        }
        return payload;
    }
}
