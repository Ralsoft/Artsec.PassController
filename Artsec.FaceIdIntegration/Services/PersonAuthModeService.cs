using Artsec.PassController.Dal;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Exceptions;
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
        var person = people.FirstOrDefault(p => p.PeopleId == personId);
        if (person is null)
            throw new AuthModeNotFoundExeption(personId);
        return (AuthMode)person.AuthMode;
    }
}
