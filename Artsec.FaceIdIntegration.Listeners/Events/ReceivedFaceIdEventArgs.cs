using Artsec.PassController.Domain.Messages;
using System.Net;

namespace Artsec.PassController.Listeners.Events;

public class ReceivedFaceIdEventArgs
{
    public FaceIdMessage Message { get; set; } = new();
}