namespace Artsec.PassController.Domain.Requests;

public class PassRequestWithPersonId : PassRequest
{
    public string PersonId { get; set; } = string.Empty;
}
