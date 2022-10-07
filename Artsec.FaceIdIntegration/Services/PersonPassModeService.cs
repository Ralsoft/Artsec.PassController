using Artsec.PassController.Dal;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class PersonAuthModeService : IPersonAuthModeService
{
    private readonly PassControllerDbContext _dbContext;

    public PersonAuthModeService(PassControllerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AuthMode> GetPersonAuthModeAsync(int personId)
    {
        var people = await _dbContext.People.GetAsync();
        return (AuthMode)people.First(p => p.PeopleId == personId).AuthMode;
    }
}
