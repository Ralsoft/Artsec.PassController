using Artsec.PassController.Dal;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using System.Diagnostics;

namespace Artsec.PassController.Services;

internal class RequestsLoggingService : IRequestsLoggingService
{
    private readonly PassControllerDbContext _dbContext;
    private readonly ILogger<RequestsLoggingService> _logger;

    public RequestsLoggingService(PassControllerDbContext dbContext, ILogger<RequestsLoggingService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task Log(PassRequest request)
    {
        if (request.IsValid && request.FaceId is not null)
        {
            var device = await _dbContext.Devices.GetByIdAsync(request.DeviceId);
            if (device is not null)
            {
                var sw = new Stopwatch();
                //sw.Start();
                //await _dbContext.Procedures.InsertDeviceEventAsync(request.ValidCode, device.ControllerId, request.Channel, request.FaceId, request.CreationTime);
                //sw.Stop();
                //_logger?.LogInformation($"InserstDeviceEvent exec time: {sw.ElapsedMilliseconds} ms");
                _logger?.LogInformation($"InserstDeviceEvent disabled");
            }
            else
            {
                _logger?.LogWarning($"Девайс с Id {request.DeviceId} не найден в базе");
            }
        }
    }
}
