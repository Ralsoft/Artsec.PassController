using Artsec.PassController.Domain.CRC;
using Artsec.PassController.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController.Domain;

public class ArtonitSePackager
{
    public ArtonitSePackager()
    {
        Args = new List<byte>();
    }
    public ArtonitSePackager(ArtonitSeCommand command) : this()
    {
        Command = command;
    }

    public ArtonitSeCommand Command { get; set; }
    public List<byte> Args { get; }

    public void AddArg(byte arg)
    {
        Args.Add(arg);
    }
    public void AddArgs(byte[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            Args.Add(args[i]);
        }
    }
    public void AddIp(string ipAddress)
    {
        IPAddress address = IPAddress.Parse(ipAddress);
        Byte[] bytes = address.GetAddressBytes();
        for (int i = 0; i < bytes.Length; i++)
        {
            Args.Add(bytes[i]);
        }
    }
    public void AddPort(int port)
    {
        byte[] intBytes = BitConverter.GetBytes(port);
        Array.Reverse(intBytes);
        byte[] result = intBytes;
        Args.Add(result[2]);
        Args.Add(result[3]);
    }

    public byte[] Pack()
    {
        byte argsCount = 0;
        checked
        {
            argsCount = (byte)Args.Count();
        }
        byte[] pack = new byte[1 + 1 + argsCount]; // Длина аргументов + команда + аргументы

        pack[0] = argsCount;
        pack[1] = Command.ConvertToByte();
        for (int i = 0; i < argsCount; i++)
        {
            pack[i + 2] = Args[i];
        }

        Crc crc = new Crc(CrcStdParams.StandartParameters[CrcAlgorithms.Crc16X25]);
        var ckeckSum = crc.ComputeHash(pack);

        var checkedPack = new byte[pack.Length + 2];
        pack.CopyTo(checkedPack, 0);
        checkedPack[pack.Length + 0] = ckeckSum[0];
        checkedPack[pack.Length + 1] = ckeckSum[1];


        return checkedPack;
    }

}
