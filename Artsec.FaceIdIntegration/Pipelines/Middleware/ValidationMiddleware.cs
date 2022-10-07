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
            AuthMode = payload.AuthMode,
            RfidPersonId = payload.RfidPersonId,
            Rfid = payload.Rfid,
        };
        int validCode = await _validationService.ValidatePassAsync(payload);
        result.IsValid = validCode == 0;
        result.ValidCode = validCode;

        return result;
    }
}
