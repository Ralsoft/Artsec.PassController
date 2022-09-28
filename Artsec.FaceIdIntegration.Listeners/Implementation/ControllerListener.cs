namespace Artsec.PassController.Listeners.Implementation;

public class ControllerListener : IListener
{
    public string SourceName => "Contoller";
    public string SourceType => "UDP";

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
