using Artsec.PassController.Domain.Enums;

namespace Artsec.PassController.Services.Interfaces;

public interface IPersonPassModeService
{
    Task<PersonPassMode> GetPersonPassModeAsync(string personId);
}
