using Artsec.PassController.Domain.Requests;

namespace Artsec.PassController.Services.Interfaces;

public interface IRequestsLoggingService
{
    Task Log(PassRequestWithValidation request);
}
