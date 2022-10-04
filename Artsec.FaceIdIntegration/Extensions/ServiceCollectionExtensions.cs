using Artsec.PassController.Configs;
using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Pipelines;
using Artsec.PassController.Services;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ICommandSender, CommandSender>();
        services.AddSingleton<IPersonPassModeService, PersonPassModeService>();
        services.AddSingleton<IPersonService, PersonService>();
        services.AddSingleton<IRequestsLoggingService, RequestsLoggingService>();
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IPassPointService, PassPointService>();

        services.AddSingleton<IInputAggregator, ListenersAggregator>();

        return services;
    }

    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.AddSingleton<PassRequestPipeline>();

        return services;
    }
    public static IServiceCollection AddListeners(this IServiceCollection services)
    {
        services.AddSingleton<FaceIdListener>();
        services.AddSingleton<ControllerListener>();

        return services;
    }
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<WorkerConfigurations>(s => config.Get<WorkerConfigurations>());
        services.AddTransient<ControllerListenerConfiguration>(s => config.Get<WorkerConfigurations>().ControllerListenerConfiguration);
        services.AddTransient<FaceIdListenerConfiguration>(s => config.Get<WorkerConfigurations>().FaceIdListenerConfiguration);

        return services;
    }
}
