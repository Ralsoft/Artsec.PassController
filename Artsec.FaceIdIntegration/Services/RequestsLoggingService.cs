using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class RequestsLoggingService : IRequestsLoggingService
{
    // TODO: Add DbContext
    public Task Log(PassRequestWithValidation request)
    {
        return Task.CompletedTask;  
    }
}
