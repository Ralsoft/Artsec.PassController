namespace Artsec.PassController.Domain.Exceptions;

[Serializable]
public class ControllerNotFoundException : Exception
{
	public ControllerNotFoundException() { }
	public ControllerNotFoundException(string message) : base(message) { }
	public ControllerNotFoundException(string message, Exception inner) : base(message, inner) { }
	protected ControllerNotFoundException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}