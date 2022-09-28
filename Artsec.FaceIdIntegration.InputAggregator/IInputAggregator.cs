using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.InputAggregator;

public interface IInputAggregator
{
    event EventHandler<EventArgs> InputReceived;
}
