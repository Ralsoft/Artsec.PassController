using Artsec.PassController.Domain.Requests;

namespace Artsec.PassController.Services.Interfaces;

public interface IValidationService
{
    Task<bool> ValidatePassAsync(PassRequest payload);
}
