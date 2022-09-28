using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Listeners.Implementation;

internal class FaceIdListener : IListener
{
    public string SourceName => "FaceId";
    public string SourceType => "HTTP";


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
