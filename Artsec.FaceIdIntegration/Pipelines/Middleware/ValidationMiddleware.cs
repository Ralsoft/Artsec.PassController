using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Middlewares.Interfaces;

namespace Artsec.PassController.Pipelines;

internal class ValidationMiddleware : IPipelineMiddleware<PassRequestWithPersonId, PassRequestWithValidation>
{
    private readonly IValidationService _validationService;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(IValidationService validationService, ILogger<ValidationMiddleware> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<PassRequestWithValidation> InvokeAsync(PassRequestWithPersonId payload)
    {

        var result = new PassRequestWithValidation(payload);
        int validCode = await _validationService.ValidatePassAsync(payload);
        result.IsValid = validCode is 50 or 148;
        result.ValidCode = validCode;

        return result;
    }
}
