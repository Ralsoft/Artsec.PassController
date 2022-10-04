using Artsec.PassController.Domain;
using Artsec.PassController.Listeners.Configurations;

namespace Artsec.PassController.Configs;

internal class WorkerConfigurations
{
    public string ConnectionString { get; set; } = string.Empty;
    public Dictionary<string, Controller> Controllers { get; set; } = new();
    public Dictionary<string, int> CamIdToDevId { get; set; } = new();
    public ControllerListenerConfiguration ControllerListenerConfiguration { get; set; } = new();
    public FaceIdListenerConfiguration FaceIdListenerConfiguration { get; set; } = new();
}
