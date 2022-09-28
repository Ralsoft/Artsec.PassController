namespace Artsec.PassController.Validation;

public interface IValidator
{
    event EventHandler<EventArgs> RequestValidated;
}
