namespace Artsec.PassController.Domain.Exceptions;

public class AuthModeNotFoundException : Exception
{
    public int PersonId { get; }
    public AuthModeNotFoundException(int personId) : base($"AuthMode for PersonId: {personId} not found")
    {
        PersonId = personId;
    }
}