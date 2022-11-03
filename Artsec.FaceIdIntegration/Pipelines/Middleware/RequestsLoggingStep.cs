using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Nodes.Steps.Interfaces;

namespace Artsec.PassController.Pipelines.Middleware;

internal class RequestsLoggingStep : IPipeStep<PassRequest>
{
    private readonly IRequestsLoggingService _requestsLoggingService;

    public RequestsLoggingStep(IRequestsLoggingService requestsLoggingService)
    {
        _requestsLoggingService = requestsLoggingService;
    }


    public async Task<PassRequest> ExecuteStepAsync(PassRequest payload)
    {
        await _requestsLoggingService.Log(payload);
        return payload;
    }
}
