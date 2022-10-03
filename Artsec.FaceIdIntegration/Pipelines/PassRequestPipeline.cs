using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Pipelines.Middleware;
using Artsec.PassController.Services.Interfaces;
using PipeLight;

namespace Artsec.PassController.Pipelines;

public class PassRequestPipeline : Pipeline<PassRequestWithPersonId>
{
	public PassRequestPipeline(IServiceProvider serviceProvider)
	{
		this
		.AddMiddleware(new ValidationMiddleware(serviceProvider.GetRequiredService<IValidationService>()))
		.AddMiddleware(new CommandSenderMiddleware(serviceProvider.GetRequiredService<ICommandSender>()))
		.AddMiddleware(new RequestsLoggingMiddleware(serviceProvider.GetRequiredService<IRequestsLoggingService>()));
	}
}
