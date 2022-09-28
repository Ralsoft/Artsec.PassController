using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class ValidationService : IValidationService
{
    public Task<bool> ValidatePassAsync(Domain.Requests.PassRequestWithMode payload)
    {
        throw new NotImplementedException();
    }
}
