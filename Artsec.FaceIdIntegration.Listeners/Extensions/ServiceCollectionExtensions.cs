using Artsec.PassController.Listeners.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Artsec.PassController.Listeners.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddListeners(this IServiceCollection services)
    {
        services.AddSingleton<FaceIdListener>();
        services.AddSingleton<ControllerListener>();

        return services;
    }
}
