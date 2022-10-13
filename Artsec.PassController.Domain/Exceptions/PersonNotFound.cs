namespace Artsec.PassController.Domain.Exceptions;

public class PersonNotFoundException : Exception
{
	public string Identifier { get; }
	public PersonNotFoundException(string identifier) : base($"Person with identifier: {identifier} not found")
	{
		Identifier = identifier;
	}
}
