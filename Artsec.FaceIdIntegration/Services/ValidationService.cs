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
        int rfidValidation = 0;
        int faceValidation = 0;
        switch (payload.AuthMode)
        {
            case AuthMode.None:
                return 0;

            case AuthMode.RequaredRfid:
                if (payload.Rfid is not null)
                    return await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);
                else
                    return 146;

            case AuthMode.RequaredRfidAndFaceId:
                if (payload.RfidPersonId != payload.FaceIdPersonId)
                    return 147;
                rfidValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);
                return rfidValidation;

            case AuthMode.RequaredRfidAndAnyFaceId:
                if (payload.FaceId is null)
                    return 146;
                rfidValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);
                faceValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);
                if (rfidValidation == 50)
                {
                    if (payload.FaceIdPersonId != payload.RfidPersonId)
                        return 148;
                    return 50;
                }
                else
                    return rfidValidation != 0 ? rfidValidation : faceValidation;

            case AuthMode.RequaredFaceId:
                if (payload.FaceId is not null)
                    return await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);
                else
                    return 146;

            case AuthMode.AnyIdentifier:

                if (payload.Rfid is not null)
                    rfidValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.Rfid);
                if (rfidValidation == 50)
                    return 50;
                if (payload.FaceId is not null)
                    faceValidation = await _dbContext.Procedures.ValidatePass(payload.DeviceId, payload.FaceId);
                if (faceValidation == 50)
                    return 50;
                return rfidValidation;

            default:
                return -1;
        }

    }
}
