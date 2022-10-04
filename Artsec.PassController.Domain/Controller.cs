namespace Artsec.PassController.Domain;

public class Controller
{
    public string Name { get; set; } = string.Empty;
    public string Ip { get; set; }
    public int Port { get; set; }
    public bool IsOnline { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, int> Channels { get; set; } = new();
}
