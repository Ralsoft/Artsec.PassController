using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
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
    private readonly PassRequestPipeline _passRequestPipeline;

    public event EventHandler<PassRequestWithPersonId> InputReceived;

    public ListenersAggregator(ControllerListener controllerListener, FaceIdListener faceIdListener)
    {
        _controllerListener = controllerListener;
        _faceIdListener = faceIdListener;
        _controllerListener.DataReceived += OnControllerListeneDataReceived;
        _faceIdListener.DataReceived += OnFaceIdListeneDataReceived;
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
            _ = _passRequestPipeline.PushAsync(request);
            _requests.TryRemove(key, out _);
        }
    }
    private async void OnFaceIdListeneDataReceived(object? sender, ReceivedDataEventArgs e)
    {
        string passPointId = await GetPassPointIdAsync(e);
        string personId = await GetPersonIdAsync(e);
        var passMode = await GetPassModeAsync(personId);

        var request = _requests.GetOrAdd(passPointId, new PassRequestWithPersonId()
        {
            PersonId = personId,
            PassMode = passMode,
        });
        request.FaceId = e.ToString()!;
        _ = RequestProcessing(request, passPointId);
    }
    private async void OnControllerListeneDataReceived(object? sender, ReceivedDataEventArgs e)
    {
        string passPointId = await GetPassPointIdAsync(e);
        string personId = await GetPersonIdAsync(e);
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
        return await Task.FromResult(PersonPassMode.None);
    }
    private async Task<string> GetPassPointIdAsync(object code)
    {
        return await Task.FromResult("Da");
    }
    private async Task<string> GetPersonIdAsync(object code)
    {
        return await Task.FromResult("Da");
    }
}
