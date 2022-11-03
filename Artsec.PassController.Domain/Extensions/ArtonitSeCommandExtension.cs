namespace Artsec.PassController.Domain.Extensions;

public static class ArtonitSeCommandExtension
{
    public static byte ConvertToByte(this ArtonitSeCommand command)
    {
        return command switch
        {
            ArtonitSeCommand.Undefined => 0x00,
            ArtonitSeCommand.WriteData => 0x02,
            ArtonitSeCommand.ReadData => 0x03,
            ArtonitSeCommand.Answer => 0x9C,
            _ => throw new ArgumentException("Unknown command"),
        };
    }
}
