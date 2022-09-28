using Artsec.PassController.Domain.Requests;
using Artsec.PassController.Listeners.Implementation;
using Artsec.PassController.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Services
{
    internal class InputAggregator : IInputAggregator
    {
        private readonly FaceIdListener _faceIdListener;
        private readonly ControllerListener _controllerListener;

        public InputAggregator(FaceIdListener faceIdListener, ControllerListener controllerListener)
        {
            _faceIdListener = faceIdListener;
            _controllerListener = controllerListener;

            _faceIdListener.MessageReceived += OnFaceIdReceived;
            _controllerListener.MessageReceived += OnControllerReceived;
        }

        public event EventHandler<PassRequestWithMode> InputReceived;

        private void OnControllerReceived(object? sender, string e)
        {
            throw new NotImplementedException();
        }

        private void OnFaceIdReceived(object? sender, string e)
        {
            throw new NotImplementedException();
        }
    }
}
