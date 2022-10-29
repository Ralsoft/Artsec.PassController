using Artsec.PassController.Configs;
using Artsec.PassController.Dal;
using Artsec.PassController.Listeners.Configurations;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Pipelines;
using Artsec.PassController.Services;
using Artsec.PassController.Services.Interfaces;
using MQTTnet;

namespace Artsec.PassController.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<PassControllerDbContext>();
        services.AddSingleton<IConnectionProvider, DefaultConnectionProvider>();

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ICommandSender, CommandSender>();
        services.AddSingleton<IPersonAuthModeService, PersonAuthModeService>();
        services.AddSingleton<IPersonService, PersonService>();
        services.AddSingleton<IRequestsLoggingService, RequestsLoggingService>();
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IPassPointService, PassPointService>();
        services.AddSingleton<IMqttService, MqttService>();
        services.AddSingleton<MqttFactory>();

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
        services.Configure<ControllersConfigurations>(config.GetSection("WorkerConfigurations"));
        services.Configure<ControllerListenerConfiguration>(config.GetSection("ControllerListenerConfiguration"));
        services.Configure<FaceIdListenerConfiguration>(config.GetSection("FaceIdListenerConfiguration"));
        services.Configure<MqttConfigurations>(config.GetSection("MqttConfigurations"));

        return services;
    }
}
