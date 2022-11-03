using Artsec.PassController.Dal;
using Artsec.PassController.Dal.Models;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Diagnostics;

namespace Artsec.PassController.Services;

internal class PersonAuthModeService : IPersonAuthModeService
{
    private readonly PassControllerDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PersonAuthModeService>? _logger;

    public PersonAuthModeService(PassControllerDbContext dbContext, IMemoryCache memoryCache, ILogger<PersonAuthModeService>? logger)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _logger = logger;
    }
    public async Task<AuthMode> GetPersonAuthModeAsync(int personId)
    {
        if (!_memoryCache.TryGetValue(GetCacheKey(personId), out AuthMode cacheValue))
        {
            var sw = new Stopwatch();
            sw.Start();
            var person = await _dbContext.People.GetByIdAsync(personId);
            sw.Stop();
            _logger?.LogInformation($"Getting AuthMode from DB: {sw.Elapsed.Milliseconds} ms");

            if (person is null || person?.AuthMode is null)
                throw new AuthModeNotFoundException(personId);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(4));
            _memoryCache.Set(GetCacheKey(personId), (AuthMode)person.AuthMode, cacheEntryOptions);

            return (AuthMode)person.AuthMode;
        }

        _ = UpdateCache(personId);

        return cacheValue;
    }
    private async Task UpdateCache(int personId)
    {
        var sw = new Stopwatch();
        sw.Start();
        var newValue = await _dbContext.People.GetByIdAsync(personId);
        sw.Stop();
        _logger?.LogInformation($"Getting AuthMode from DB for cache: {sw.Elapsed.Milliseconds} ms");

        if (newValue is null || newValue?.AuthMode is null)
            return;

        if (!_memoryCache.TryGetValue(GetCacheKey(personId), out AuthMode cacheValue))
        {
            if (cacheValue != (AuthMode)newValue.AuthMode)
            {
                _memoryCache.Remove(GetCacheKey(personId));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(4));
                _memoryCache.Set(GetCacheKey(personId), (AuthMode)newValue.AuthMode, cacheEntryOptions);
            }
        }
    }
    private string GetCacheKey(int personId)
    {
        return $"authModeForPersonId{personId}";
    }
}
