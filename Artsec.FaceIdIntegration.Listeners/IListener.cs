using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Listeners;

public interface IListener
{
    string SourceName { get; }
    string SourceType { get; }
    event EventHandler<string> MessageReceived;
    void StartListen();
    void StopListen();
}
