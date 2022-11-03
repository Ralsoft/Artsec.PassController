using Artsec.PassController.Configs;
using Artsec.PassController.Domain;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Listeners;
using Artsec.PassController.Listeners.Events;
using Artsec.PassController.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;

namespace Artsec.PassController.Services;

internal class ListenersAggregator : IInputAggregator
{
    private readonly ConcurrentDictionary<int, PassRequest> _requests = new();

    private readonly ControllerListener _controllerListener;
    private readonly FaceIdListener _faceIdListener;
    private readonly ILogger<ListenersAggregator> _logger;
    private readonly IPersonAuthModeService _personPassModeService;
    private readonly IPersonService _personService;
    private readonly IPassPointService _passPointService;
    private readonly IOptions<AggregatorConfigurations> _options;

    public event EventHandler<PassRequest> InputReceived;

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

    private async Task RequestProcessing(PassRequest request, int key)
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


            
            
            if (!_requests.TryGetValue(passPointId, out var request))
            {
                request = new PassRequest()
                {
                    CreationTime = startTime,
                    FaceId = e.Message.FaceId,
                    RemoteAddress = controller.Ip,
                    RemotePort = controller.Port,
                    Channel = channel,
                    CamId = e.Message.CamId,
                    FaceIdPersonId = personId,
                    AuthMode = authMode,
                };
                _requests.TryAdd(passPointId, request);
            }
            else
            {
                request.FaceId = e.Message.FaceId;
                request.CamId = e.Message.CamId;
                request.FaceIdPersonId = personId;
            }

            _ = RequestProcessing(request, passPointId);
        }
        catch (Exception ex)
        {
            if (ex is PersonNotFoundException || ex is AuthModeNotFoundException)
            {
                _logger?.LogWarning(ex.Message);
            }
            else
            {
                _logger?.LogError(ex.Message, ex);
            }
        }
    }
    private async void OnControllerListeneMessageReceived(object? sender, ReceivedRfidEventArgs e)
    {
        var startTime = DateTime.Now;
        try
        {
            var message = RfidMessage.Parse(e.Data);
            _logger?.LogInformation($"Получен RFID: {message.RfidString}");
            int passPointId = GetPassPointId(e.RemoteIp.Address.ToString(), message.Channel);
            _logger?.LogInformation($"Получен id точкит прохода: {passPointId}");
            int personId = await GetPersonIdAsync(message.RfidString);
            _logger?.LogInformation($"Получен PersonId: {personId}");
            var authMode = await GetAuthModeAsync(personId);
            _logger?.LogInformation($"Получен режим авторизации: {authMode}");



            if (!_requests.TryGetValue(passPointId, out var request))
            {
                request = new PassRequest()
                {
                    CreationTime = startTime,
                    Rfid = message.RfidString,
                    Data = e.Data,
                    RemoteAddress = e.RemoteIp.Address.ToString(),
                    RemotePort = e.RemoteIp.Port,
                    Channel = message.Channel,
                    RfidPersonId = personId,
                    AuthMode = authMode,
                };
                _requests.TryAdd(passPointId, request);
            }
            else
            {
                request.Rfid = message.RfidString;
                request.Data = e.Data;
                request.RfidPersonId = personId;
                request.AuthMode = authMode;
            }


            _ = RequestProcessing(request, passPointId);
        }
        catch (Exception ex)
        {

            if (ex is PersonNotFoundException || ex is AuthModeNotFoundException)
            {
                _logger?.LogWarning(ex.Message);
                var message = RfidMessage.Parse(e.Data);
                int passPointId = GetPassPointId(e.RemoteIp.Address.ToString(), message.Channel);


                if (!_requests.TryGetValue(passPointId, out var request))
                {
                    request = new PassRequest()
                    {
                        CreationTime = startTime,
                        Rfid = message.RfidString,
                        Data = e.Data,
                        RemoteAddress = e.RemoteIp.Address.ToString(),
                        RemotePort = e.RemoteIp.Port,
                        Channel = message.Channel,
                        RfidPersonId = 0,
                        AuthMode = AuthMode.None,
                    };
                    _requests.TryAdd(passPointId, request);
                    _ = RequestProcessing(request, passPointId);
                }
            }
            else
            {
                _logger?.LogError(ex.Message, ex);
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
