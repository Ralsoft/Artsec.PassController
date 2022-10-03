// See https://aka.ms/new-console-template for more information

using Artsec.PassController.Listeners.Configuration;
using Artsec.PassController.Listeners.Implementation;
using Microsoft.Extensions.Logging;

var controllerListener = new ControllerListener(new ControllerListenerConfiguration() { Ip = "172.19.0.95" });
controllerListener.StartListen();


Console.ReadLine();