using Artsec.PassController.Configs;
using Artsec.PassController.Domain;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Listeners.Events;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;

namespace Artsec.PassController.Services;

internal class ListenersAggregator : IInputAggregator
{
    private readonly ConcurrentDictionary<int, PassRequestWithPersonId> _requests = new();

    private readonly ControllerListener _controllerListener;
    private readonly FaceIdListener _faceIdListener;
    private readonly ILogger<ListenersAggregator> _logger;
    private readonly IPersonAuthModeService _personPassModeService;
    private readonly IPersonService _personService;
    private readonly IPassPointService _passPointService;
    private readonly IOptions<AggregatorConfigurations> _options;

    public event EventHandler<PassRequestWithPersonId> InputReceived;

    public ListenersAggregator(
        ControllerListener controllerListener, FaceIdListener faceIdListener, ILogger<ListenersAggregator> logger,
        IPersonAuthModeService personPassModeService, IPersonService personService, IPassPointService passPointService,
        IOptions<AggregatorConfigurations> options)
    {
        _controllerListener = controllerListener;
        _faceIdListener = faceIdListener;
        _logger = logger;
        _personPassModeService = personPassModeService;
        _personService = personService;
        _passPointService = passPointService;
        _options = options;
        _controllerListener.MessageReceived += OnControllerListeneMessageReceived;
        _faceIdListener.MessageReceived += OnFaceIdListeneMessageReceived;

        _controllerListener.StartListen();
        _faceIdListener.StartListen();
    }

    private async Task RequestProcessing(PassRequestWithPersonId request, int key)
    {
        request.DeviceId = key;
        if (request.IsReadyToProcessing())
        {
            SendAndRemove(key);
        }
        else
        {
            await Task.Delay(TimeSpan.FromSeconds(_options.Value.AggregateDelaySeconds ?? 5));
            SendAndRemove(key);
        }
    }

    private void SendAndRemove(int key)
    {
        if (_requests.TryGetValue(key, out var request))
        {
            InputReceived?.Invoke(this, request);
            _requests.TryRemove(key, out _);
        }
    }
    private async void OnFaceIdListeneMessageReceived(object? sender, ReceivedFaceIdEventArgs e)
    {
        try
        {
            var startTime = DateTime.Now;
            if (e.Message.SrcAction != "FIND_PERSON")
                return;
            _logger?.LogInformation(
                $"Получен FaceId: {e.Message.FaceId}\n" +
                $"От  CamId: {e.Message.CamId}");
            int passPointId = GetPassPointIdForFaceId(int.Parse(e.Message.CamId));;
            _logger?.LogInformation($"Точка прохода: {passPointId}");
            int personId = await GetPersonIdAsync(e.Message.FaceId);
            _logger?.LogInformation($"Для него PersonId: {personId}");
            var authMode = await GetAuthModeAsync(personId);
            _logger?.LogInformation($"У него режим авторизации: {authMode}");
            var controller = _passPointService.GetControllerByDeviceId(passPointId);
            _logger?.LogInformation($"Контроллер: {controller}");
            var channel = int.Parse(controller.Channels.First(x => x.Value == passPointId).Key);
            _logger?.LogInformation($"Канал: {channel}");


            PassRequestWithPersonId request;
            if (!_requests.ContainsKey(passPointId))
            {
                var baseRequset = new PassRequest()
                {
                    CreationTime = startTime,
                    FaceId = e.Message.FaceId,
                    RemoteAddress = controller.Ip,
                    RemotePort = controller.Port,
                    Channel = channel,
                    CamId = e.Message.CamId,
                };
                request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId(baseRequset)
                {
                    FaceIdPersonId = personId,
                    AuthMode = authMode,
                });
            }
            else
            {
                request = _requests[passPointId];
                request.FaceId = e.Message.FaceId;
                request.CamId = e.Message.CamId;
                request.FaceIdPersonId = personId;
            }

            _ = RequestProcessing(request, passPointId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message);
        }
    }
    private async void OnControllerListeneMessageReceived(object? sender, ReceivedRfidEventArgs e)
    {
        try
        {
            var startTime = DateTime.Now;
            var message = RfidMessage.Parse(e.Data);
            _logger?.LogInformation($"Получен RFID: {message.RfidString}");
            int passPointId = GetPassPointId(e.RemoteIp.Address.ToString(), message.Channel);
            _logger?.LogInformation($"Получен id точкит прохода: {passPointId}");
            int personId = await GetPersonIdAsync(message.RfidString);
            _logger?.LogInformation($"Получен PersonId: {personId}");
            var authMode = await GetAuthModeAsync(personId);
            _logger?.LogInformation($"Получен режим авторизации: {authMode}");


            PassRequestWithPersonId request;
            if (!_requests.ContainsKey(passPointId))
            {

                var baseRequset = new PassRequest()
                {
                    CreationTime = startTime,
                    Rfid = message.RfidString,
                    Data = e.Data,
                    RemoteAddress = e.RemoteIp.Address.ToString(),
                    RemotePort = e.RemoteIp.Port,
                    Channel = message.Channel,
                };
                request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId(baseRequset)
                {
                    RfidPersonId = personId,
                    AuthMode = authMode,
                });
            }
            else
            {
                request = _requests[passPointId];
                request.Rfid = message.RfidString;
                request.Data = e.Data;
                request.RfidPersonId = personId;
                request.AuthMode = authMode;
            }
            
            _ = RequestProcessing(request, passPointId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message);

            if (ex is PersonNotFoundException || ex is AuthModeNotFoundException)
            {
                var message = RfidMessage.Parse(e.Data);
                int passPointId = GetPassPointId(e.RemoteIp.Address.ToString(), message.Channel);
                var baseRequset = new PassRequest()
                {
                    Rfid = message.RfidString,
                    Data = e.Data,
                    RemoteAddress = e.RemoteIp.Address.ToString(),
                    RemotePort = e.RemoteIp.Port,
                    Channel = message.Channel,
                };

                var request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId(baseRequset)
                {
                    RfidPersonId = 0,
                    AuthMode = AuthMode.None,
                });
                _ = RequestProcessing(request, passPointId);
            }
        }
    }

    private async Task<AuthMode> GetAuthModeAsync(int personId)
    {
        return await _personPassModeService.GetPersonAuthModeAsync(personId);
    }
    private int GetPassPointId(string ip, int channelNumber)
    {
        return _passPointService.GetPassPointId(ip, channelNumber);
    }
    private int GetPassPointIdForFaceId(int channelNumber)
    {
        return _passPointService.GetPassPointIdForFaceId(channelNumber);
    }
    private async Task<int> GetPersonIdAsync(string? identifier)
    {
        return await _personService.GetPersonIdAsync(identifier);
    }
}
