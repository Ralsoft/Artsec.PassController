using Artsec.PassController.Domain;
using Artsec.PassController.Listeners.Configurations;

namespace Artsec.PassController.Configs;
public class ControllersConfigurations
{
    public Dictionary<string, Controller> Controllers { get; set; } = new();
    public Dictionary<string, int> CamIdToDevId { get; set; } = new();
}
