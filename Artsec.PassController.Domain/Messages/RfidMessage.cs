using Artsec.PassController.Domain.CRC;
using Artsec.PassController.Domain.Exceptions;
using Artsec.PassController.Domain.Helpers;
using System.ComponentModel;

namespace Artsec.PassController.Domain.Messages;

public class RfidMessage
{
    public byte[] MacData { get; set; } = Array.Empty<byte>();
    public byte Length { get; set; }
    public MessageType MessageType { get; set; }
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public byte[] CheckSum { get; set; } = Array.Empty<byte>();
    public byte Channel => Body.Skip(4).Take(1).First();
    public byte[] Rfid => RfidHelper.ConvertMessageBodyToRfid(Body);
    public string RfidString => RfidHelper.RfidToString(Rfid);
    public static RfidMessage Parse(byte[] data)
    {
        var message = new RfidMessage();
        message.MacData = data.Take(8).ToArray();
        message.Length = data.Skip(8).Take(1).First();
        message.MessageType = (MessageType)data.Skip(9).Take(1).First();
        message.Body = data.Skip(10).Take(message.Length).ToArray();
        message.CheckSum = new byte[2]
        {
            data[^1],
            data[^2],
        };



        var crc = new Crc(CrcStdParams.StandartParameters[CrcAlgorithms.Crc16X25]);
        var ckeckSum = crc.ComputeHash(data.Skip(8).Take(2 + message.Length).ToArray());



        if ((message.CheckSum[0] == ckeckSum[1]) &&
           (message.CheckSum[1] == ckeckSum[0]))
        {
            return message;
        }
        else
        {
            throw new CheckSumException("Invalid check sum");
        }
    }
}
