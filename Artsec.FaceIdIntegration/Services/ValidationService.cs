using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class ValidationService : IValidationService
{
    public Task<bool> ValidatePassAsync(PassRequest payload)
    {
        throw new NotImplementedException();
    }
}
