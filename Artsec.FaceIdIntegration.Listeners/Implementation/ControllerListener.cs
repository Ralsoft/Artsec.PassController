using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Listeners.Implementation;

internal class ControllerListener : IListener
{
    public string SourceName => "Contoller";
    public string SourceType => "UDP";
    public event EventHandler<EventArgs> MessageReceived;

    public void StartListen()
    {
        throw new NotImplementedException();
    }

    public void StopListen()
    {
        throw new NotImplementedException();
    }
}
