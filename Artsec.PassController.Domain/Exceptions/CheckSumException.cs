using System.Runtime.Serialization;

namespace Artsec.PassController.Domain.Exceptions
{
    [Serializable]
    internal class CheckSumException : Exception
    {
        public CheckSumException()
        {
        }

        public CheckSumException(string? message) : base(message)
        {
        }

        public CheckSumException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected CheckSumException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}