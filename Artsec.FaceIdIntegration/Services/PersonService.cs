using Artsec.PassController.Dal;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class PersonService : IPersonService
{
    private readonly PassControllerDbContext _dbContext;

    public PersonService(PassControllerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> GetPersonIdAsync(string? identifier)
    {
        var cards = await _dbContext.Cards.GetAsync();
        var card = cards.First(c => c.CardId == identifier);
        return card.People.PeopleId;
    }
}
