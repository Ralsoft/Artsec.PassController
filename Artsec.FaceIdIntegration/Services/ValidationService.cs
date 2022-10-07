using Artsec.PassController.Dal;
using Artsec.PassController.Domain.Enums;
using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class ValidationService : IValidationService
{
    private readonly PassControllerDbContext _dbContext;
    private readonly IPersonService _personService;

    public ValidationService(PassControllerDbContext dbContext, IPersonService personService)
    {
        _dbContext = dbContext;
        _personService = personService;
    }
    public async Task<int> ValidatePassAsync(PassRequestWithPersonId payload)
    {
        int rfidValidation;
        int faceValidation;
        switch (payload.AuthMode)
        {
            case AuthMode.None:
                return -1;

            case AuthMode.RequaredRfid:
                return await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);

            case AuthMode.RequaredRfidAndFaceId:
                if (payload.RfidPersonId != payload.FaceIdPersonId)
                    return 147;
                rfidValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);
                return rfidValidation;

            case AuthMode.RequaredRfidAndAnyFaceId:
                if (payload.FaceId == null)
                    return 146;
                rfidValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);
                faceValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);
                if (rfidValidation == 0 || faceValidation == 0)
                    return 0;
                else
                    return rfidValidation == 0 ? rfidValidation : faceValidation;

            case AuthMode.RequaredFaceId:
                return await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);

            case AuthMode.AnyIdentifier:
                rfidValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);
                if (rfidValidation == 0)
                    return 0;
                faceValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);
                if (faceValidation == 0)
                    return 0;
                return rfidValidation;

            default:
                return -1;
        }

    }
}
