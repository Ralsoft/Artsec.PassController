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
        services.AddSingleton<IInputAggregator, InputAggregator>();

        return services;
    }

    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.AddSingleton<PassRequestPipeline>();

        return services;
    }
}
