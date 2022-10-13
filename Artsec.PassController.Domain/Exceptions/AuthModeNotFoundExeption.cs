namespace Artsec.PassController.Domain.Exceptions;

public class AuthModeNotFoundExeption : Exception
{
    public int PersonId { get; }
    public AuthModeNotFoundExeption(int personId) : base($"AuthMode for PersonId: {personId} not found")
    {
        PersonId = personId;
    }
}