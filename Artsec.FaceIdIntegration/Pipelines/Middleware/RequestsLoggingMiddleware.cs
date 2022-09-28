using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Middlewares.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Pipelines.Middleware;

internal class RequestsLoggingMiddleware : IPipelineMiddleware<PassRequestWithValidation, PassRequestWithValidation>
{
    private readonly IRequestsLoggingService _requestsLoggingService;

    public RequestsLoggingMiddleware(IRequestsLoggingService requestsLoggingService)
    {
        _requestsLoggingService = requestsLoggingService;
    }

    public async Task<PassRequestWithValidation> InvokeAsync(PassRequestWithValidation payload)
    {
        await _requestsLoggingService.Log(payload);
        return payload;
    }
}
