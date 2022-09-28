namespace Artsec.PassController.Listeners.Implementation;

public class FaceIdListener : IListener
{
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
