using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class PersonPassModeService : IPersonPassModeService
{
    public Task<PersonPassMode> GetPersonPassModeAsync(string personId)
    {
        throw new NotImplementedException();
    }
}
