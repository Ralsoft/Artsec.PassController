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
            await _commandSender.SendAsync();
        }
        return await Task.FromResult(payload);
    }
}
