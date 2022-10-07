using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Messages;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Listeners.Events;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Services.Interfaces;
using System.Collections.Concurrent;

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

    public event EventHandler<PassRequestWithPersonId> InputReceived;

    public ListenersAggregator(
        ControllerListener controllerListener, FaceIdListener faceIdListener, ILogger<ListenersAggregator> logger,
        IPersonAuthModeService personPassModeService, IPersonService personService, IPassPointService passPointService)
    {
        _controllerListener = controllerListener;
        _faceIdListener = faceIdListener;
        _logger = logger;
        _personPassModeService = personPassModeService;
        _personService = personService;
        _passPointService = passPointService;
        _controllerListener.DataReceived += OnControllerListeneDataReceived;
        _faceIdListener.MessageReceived += OnFaceIdListeneDataReceived;

        _controllerListener.StartListen();
        _faceIdListener.StartListen();
    }

    private async Task RequestProcessing(PassRequestWithPersonId request, int key)
    {
        if (request.IsReadyToProcessing())
        {
            SendAndRemove(key);
        }
        else
        {
            await Task.Delay(5000);
            SendAndRemove(key);
        }
    }

    private void SendAndRemove(int key)
    {
        if (_requests.TryGetValue(key, out var request))
        {
            _logger?.LogInformation($"Inner FaceId = {request.FaceId}");
            InputReceived?.Invoke(this, request);
            _requests.TryRemove(key, out _);
        }
    }
    private async void OnFaceIdListeneDataReceived(object? sender, ReceivedFaceIdEventArgs e)
    {
        try
        {
            _logger?.LogInformation(e.Message.SrcAction);
            if (e.Message.SrcAction != "FIND_PERSON")
                return;
            int passPointId = GetPassPointIdForFaceId(int.Parse(e.Message.CamId));
            int personId = await GetPersonIdAsync(e.Message.FaceId);
            var passMode = await GetAuthModeAsync(personId);

            var request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId()
            {
                FaceIdPersonId = personId,
                AuthMode = passMode,
            });
            request.FaceId = e.Message.FaceId;
            _ = RequestProcessing(request, passPointId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message);
        }
    }
    private async void OnControllerListeneDataReceived(object? sender, ReceivedRfidEventArgs e)
    {
        try
        {
            var message = RfidMessage.Parse(e.Data);
            int passPointId = GetPassPointId(e.RemoteIp.Address.ToString(), message.Channel);
            int personId = await GetPersonIdAsync(e.Rfid);
            var authMode = await GetAuthModeAsync(personId);

            var request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId()
            {
                RfidPersonId = personId,
                AuthMode = authMode,
            });
            request.Rfid = e.ToString()!;
            request.Data = e.Data;
            request.RemoteAddress = e.RemoteIp.Address.ToString();
            request.RemotePort = e.RemoteIp.Port;
            request.Channel = message.Channel;
            _ = RequestProcessing(request, passPointId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message);
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
