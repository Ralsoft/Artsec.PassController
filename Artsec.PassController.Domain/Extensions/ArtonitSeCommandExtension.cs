namespace Artsec.PassController.Domain.Extensions;

public static class ArtonitSeCommandExtension
{
    public static byte ConvertToByte(this ArtonitSeCommand command)
    {
        switch (command)
        {
            case ArtonitSeCommand.Undefined:
                return 0x00;
            case ArtonitSeCommand.WriteData:
                return 0x02;
            case ArtonitSeCommand.ReadData:
                return 0x03;
            case ArtonitSeCommand.Answer:
                return 0x9C;
            default:
                throw new ArgumentException("Unknown command");
        }
    }
}
