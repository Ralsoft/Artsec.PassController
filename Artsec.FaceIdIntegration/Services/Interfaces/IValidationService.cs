using Artsec.PassController.Domain.Requests;

namespace Artsec.PassController.Services.Interfaces;

public interface IValidationService
{
    Task<int> ValidatePassAsync(PassRequestWithPersonId payload);
}
