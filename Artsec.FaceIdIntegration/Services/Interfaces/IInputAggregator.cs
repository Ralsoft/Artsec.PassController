using Artsec.PassController.Domain.Requests;

namespace Artsec.PassController.Services.Interfaces;

public interface IInputAggregator
{
    event EventHandler<PassRequestWithMode> InputReceived;
}
