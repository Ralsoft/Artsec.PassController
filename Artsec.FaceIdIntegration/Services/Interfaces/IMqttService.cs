namespace Artsec.PassController.Services.Interfaces;

public interface IMqttService
{
    Task SendMessageAsync(string message);
}
