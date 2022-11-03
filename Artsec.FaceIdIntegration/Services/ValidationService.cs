using Artsec.PassController.Dal;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Artsec.PassController.Services;

internal class ValidationService : IValidationService
{
    private readonly PassControllerDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ValidationService>? _logger;

    public ValidationService(PassControllerDbContext dbContext, IMemoryCache memoryCache, ILogger<ValidationService>? logger)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _logger = logger;
    }
    public async Task<int> ValidatePassAsync(PassRequestWithPersonId payload)
    {
        int rfidValidation = 0;
        int faceValidation = 0;
        switch (payload.AuthMode)
        {
            case AuthMode.None:
                return 0;

            case AuthMode.RequaredRfid:
                if (payload.Rfid is not null)
                    return await ValidatePassAsync(payload.DeviceId, payload.FaceId);
                else
                    return 146;

            case AuthMode.RequaredRfidAndFaceId:
                if (payload.RfidPersonId != payload.FaceIdPersonId)
                    return 147;
                rfidValidation = await ValidatePassAsync(payload.DeviceId, payload.Rfid);
                return rfidValidation;

            case AuthMode.RequaredRfidAndAnyFaceId:
                if (payload.FaceId is null)
                    return 146;
                rfidValidation = await ValidatePassAsync(payload.DeviceId, payload.Rfid);
                faceValidation = await ValidatePassAsync(payload.DeviceId, payload.FaceId);
                if (rfidValidation == 50)
                {
                    if (payload.FaceIdPersonId != payload.RfidPersonId)
                        return 148;
                    return 50;
                }
                else
                    return rfidValidation != 0 ? rfidValidation : faceValidation;

            case AuthMode.RequaredFaceId:
                if (payload.FaceId is not null)
                    return await ValidatePassAsync(payload.DeviceId, payload.FaceId);
                else
                    return 146;

            case AuthMode.AnyIdentifier:

                if (payload.Rfid is not null)
                    rfidValidation = await ValidatePassAsync(payload.DeviceId, payload.Rfid);
                if (rfidValidation == 50)
                    return 50;
                if (payload.FaceId is not null)
                    faceValidation = await ValidatePassAsync(payload.DeviceId, payload.FaceId);
                if (faceValidation == 50)
                    return 50;
                return rfidValidation;

            default:
                return -1;
        }
    }
    private async Task<int> ValidatePassAsync(int deviceId, string identifier)
    {
        if (!_memoryCache.TryGetValue(GetCacheKey(deviceId, identifier), out int cacheValue))
        {
            var sw = new Stopwatch();
            sw.Start();
            var newValue = await _dbContext.Procedures.ValidatePass(deviceId, identifier);
            sw.Stop();
            _logger?.LogInformation($"Validating Identifier:{identifier} for Device:{deviceId}: {sw.Elapsed.Milliseconds} ms");


            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(4));
            _memoryCache.Set(GetCacheKey(deviceId, identifier), newValue, cacheEntryOptions);
            return newValue;
        }

        _ = UpdateCache(deviceId, identifier);

        return cacheValue;
    }
    private async Task UpdateCache(int deviceId, string identifier)
    {
        var sw = new Stopwatch();
        sw.Start();
        var newValue = await _dbContext.Procedures.ValidatePass(deviceId, identifier);
        sw.Stop();
        _logger?.LogInformation($"Validating Identifier:{identifier} for Device:{deviceId} for cache: {sw.Elapsed.Milliseconds} ms");


        if (!_memoryCache.TryGetValue(GetCacheKey(deviceId, identifier), out int cacheValue))
        {
            if (cacheValue != newValue)
            {
                _memoryCache.Remove(GetCacheKey(deviceId, identifier));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(4));
                _memoryCache.Set(GetCacheKey(deviceId, identifier), newValue, cacheEntryOptions);
            }
        }
    }

    private string GetCacheKey(int deviceId, string identifier)
    {
        return $"validationForDevice{deviceId}ForIdentifier{identifier}";
    }
}
