using Artsec.PassController.Domain.Enums;

namespace Artsec.PassController.Services.Interfaces;

public interface IPersonAuthModeService
{
    Task<AuthMode> GetPersonAuthModeAsync(int personId);
}
