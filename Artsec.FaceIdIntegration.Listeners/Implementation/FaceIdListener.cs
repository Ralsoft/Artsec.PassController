namespace Artsec.PassController.Listeners.Implementation;

public class FaceIdListener
{
    public event EventHandler<ReceivedDataEventArgs> DataReceived;
    public string SourceName => "FaceId";
    public string SourceType => "HTTP";


    public event EventHandler<string> MessageReceived;

    public void StartListen()
    {
        throw new NotImplementedException();
    }

    public void StopListen()
    {
        throw new NotImplementedException();
    }
}
