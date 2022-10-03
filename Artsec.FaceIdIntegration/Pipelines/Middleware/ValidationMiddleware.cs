using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services;
using Artsec.PassController.Services.Interfaces;
using PipeLight.Middlewares.Interfaces;

namespace Artsec.PassController.Pipelines;

internal class ValidationMiddleware : IPipelineMiddleware<PassRequestWithPersonId, PassRequestWithValidation>
{
    private readonly IValidationService _validationService;

    public ValidationMiddleware(IValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task<PassRequestWithValidation> InvokeAsync(PassRequestWithPersonId payload)
    {
        var result = new PassRequestWithValidation()
        {
            FaceId = payload.FaceId,
            PassMode = payload.PassMode,
            PersonId = payload.PersonId,
            Rfid = payload.Rfid,
        };
        bool isValid = await _validationService.ValidatePassAsync(payload); 
        result.IsValid = isValid;

        return result;
    }
}
