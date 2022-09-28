using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class RequestsLoggingService : IRequestsLoggingService
{
    public Task Log(PassRequestWithValidation request)
    {
        throw new NotImplementedException();
    }
}
