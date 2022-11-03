using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Nodes.Steps.Interfaces;

namespace Artsec.PassController.Pipelines;

internal class ValidationStep : IPipeStep<PassRequest>
{
    private readonly IValidationService _validationService;
    private readonly ILogger<ValidationStep> _logger;

    public ValidationStep(IValidationService validationService, ILogger<ValidationStep> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }


    public async Task<PassRequest> ExecuteStepAsync(PassRequest payload)
    {

        int validCode = await _validationService.ValidatePassAsync(payload);
        payload.IsValid = validCode is 50 or 148;
        payload.ValidCode = validCode;

        return payload;
    }
}
