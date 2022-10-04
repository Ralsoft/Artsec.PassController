using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Listeners.Events;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Pipelines;
using Artsec.PassController.Services.Interfaces;
using System.Collections.Concurrent;
using System.Timers;

namespace Artsec.PassController.Services;

internal class ListenersAggregator : IInputAggregator
{
    private readonly ConcurrentDictionary<string, PassRequestWithPersonId> _requests = new();

    private readonly ControllerListener _controllerListener;
    private readonly FaceIdListener _faceIdListener;
    private readonly IPersonPassModeService _personPassModeService;
    private readonly IPersonService _personService;
    private readonly IPassPointService _passPointService;

    public event EventHandler<PassRequestWithPersonId> InputReceived;

    public ListenersAggregator(
        ControllerListener controllerListener, FaceIdListener faceIdListener, 
        IPersonPassModeService personPassModeService, IPersonService personService, IPassPointService passPointService)
    {
        _controllerListener = controllerListener;
        _faceIdListener = faceIdListener;
        _personPassModeService = personPassModeService;
        _personService = personService;
        _passPointService = passPointService;
        _controllerListener.DataReceived += OnControllerListeneDataReceived;
        _faceIdListener.MessageReceived += OnFaceIdListeneDataReceived;
    }

    private async Task RequestProcessing(PassRequestWithPersonId request, string key)
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

    private void SendAndRemove(string key)
    {
        if (_requests.TryGetValue(key, out var request))
        {
            InputReceived?.Invoke(this, request);
            _requests.TryRemove(key, out _);
        }
    }
    private async void OnFaceIdListeneDataReceived(object? sender, ReceivedFaceIdEventArgs e)
    {
        string passPointId = GetPassPointIdForFaceId(e.Message.CamId);
        string personId = await GetPersonIdAsync(e.Message.FaceId);
        var passMode = await GetPassModeAsync(personId);

        var request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId()
        {
            PersonId = personId,
            PassMode = passMode,
        });
        request.FaceId = e.ToString()!;
        _ = RequestProcessing(request, passPointId);
    }
    private async void OnControllerListeneDataReceived(object? sender, ReceivedRfidEventArgs e)
    {
        string passPointId = GetPassPointId(e.RemoteIp.Address.ToString());
        string personId = await GetPersonIdAsync(e.Rfid);
        var passMode = await GetPassModeAsync(personId);

        var request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId()
        {
            PersonId = personId,
            PassMode = passMode,
        });
        request.Rfid = e.ToString()!;
        _ = RequestProcessing(request, passPointId);
    }

    private async Task<PersonPassMode> GetPassModeAsync(string personId)
    {
        return await _personPassModeService.GetPersonPassModeAsync(personId);
    }
    private string GetPassPointId(string ip)
    {
        return _passPointService.GetPassPointId(ip);
    }
    private string GetPassPointIdForFaceId(int channelNumber)
    {
        return _passPointService.GetPassPointIdForFaceId(channelNumber);
    }
    private async Task<string> GetPersonIdAsync(string code)
    {
        return await _personService.GetPersonIdAsync(code);
    }
}
