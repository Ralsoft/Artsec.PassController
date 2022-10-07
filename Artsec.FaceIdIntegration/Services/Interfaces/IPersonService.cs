namespace Artsec.PassController.Services.Interfaces;

public interface IPersonService
{
    Task<int> GetPersonIdAsync(string? identifier);
}
