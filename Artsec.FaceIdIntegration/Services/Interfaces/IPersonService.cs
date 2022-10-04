namespace Artsec.PassController.Services.Interfaces;

public interface IPersonService
{
    Task<string> GetPersonIdAsync(string id);
}
