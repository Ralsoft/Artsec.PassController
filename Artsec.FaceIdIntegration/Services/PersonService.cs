using Artsec.PassController.Services.Interfaces;

namespace Artsec.PassController.Services;

internal class PersonService : IPersonService
{
    public Task<string> GetFaceIdPersonIdAsync(string faceId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetRfidPersonIdAsync(string rfid)
    {
        throw new NotImplementedException();
    }
}
