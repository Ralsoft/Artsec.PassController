using Artsec.PassController.Dal;
using Artsec.PassController.Dal.Models;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Artsec.PassController.Services;

public class PersonService : IPersonService
{
    private readonly PassControllerDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PersonService>? _logger;

    public PersonService(PassControllerDbContext dbContext, IMemoryCache memoryCache, ILogger<PersonService>? logger)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _logger = logger;
    }
    public async Task<int> GetPersonIdAsync(string? identifier)
    {
        if (!_memoryCache.TryGetValue(GetCacheKey(identifier), out int cacheValue))
        {
            var sw = new Stopwatch();
            sw.Start();
            var card = await _dbContext.Cards.GetByIdAsync(identifier);
            sw.Stop();
            _logger?.LogInformation($"Getting PersonId from DB: {sw.Elapsed.Milliseconds} ms");

            if (card is null || card?.PeopleId is null)
                throw new PersonNotFoundException(identifier);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(4));
            _memoryCache.Set(GetCacheKey(identifier), card.People!.PeopleId, cacheEntryOptions);

            return card.People.PeopleId;
        }

        _ = UpdateCache(identifier);

        return cacheValue;
    }

    private async Task UpdateCache(string identifier)
    {
        var sw = new Stopwatch();
        sw.Start();
        var newValue = await _dbContext.Cards.GetByIdAsync(identifier);
        sw.Stop();
        _logger?.LogInformation($"Getting PersonId from DB for cache: {sw.Elapsed.Milliseconds} ms");

        if (newValue is null || newValue?.PeopleId is null)
            return;

        if (!_memoryCache.TryGetValue(GetCacheKey(identifier), out int cacheValue))
        {
            if (cacheValue != newValue.People.PeopleId)
            {
                _memoryCache.Remove(GetCacheKey(identifier));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(4));
                _memoryCache.Set(GetCacheKey(identifier), newValue.People.PeopleId, cacheEntryOptions);
            }
        }
    }

    private string GetCacheKey(string? identifier)
    {
        return $"personeIdWithCardIdentifier{identifier}";
    }
}
