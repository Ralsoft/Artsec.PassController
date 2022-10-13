using Artsec.PassController.Dal;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

public class PersonService : IPersonService
{
    private readonly PassControllerDbContext _dbContext;

    public PersonService(PassControllerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> GetPersonIdAsync(string? identifier)
    {
        var cards = await _dbContext.Cards.GetAsync();
        var card = cards.FirstOrDefault(c => c.CardId == identifier);
        if (card is null)
            throw new PersonNotFoundException(identifier);
        return card.People.PeopleId;
    }
}
