using System.Collections;
using System.Text;

namespace Artsec.PassController.Domain.Helpers;
public static class BitArrayExtension
{
    public static BitArray Reverse(this BitArray array)
    {
        int length = array.Length;
        int mid = (length / 2);
        var reversedArr = new BitArray(length);

        for (int i = 0; i < mid; i++)
        {
            bool bit = array[i];
            reversedArr[i] = array[length - i - 1];
            reversedArr[length - i - 1] = bit;
        }
        return reversedArr;
    }
}
public static class RfidHelper
{
    public static byte[] ConvertMessageBodyToRfid(byte[] data)
    {
        if (data.Length < 4)
        {
            throw new ArgumentException("Body length must be more then 4");
        }

        var newData = new byte[5];
        for (int i = 0; i < 4; i++)
        {
            var biteArr = new BitArray(new byte[] { data[i] });
            newData[i] = ConvertToByte(biteArr.Reverse());
        }
        newData[4] = 0x1A;
        return newData;
    }
    public static string RfidToString(byte[] rfid, string delimiter = "")
    {
        StringBuilder sb = new StringBuilder();
        foreach (var hex in rfid)
        {
            if (hex < 16)
            {
                sb.Append("0");
            }
            sb.Append(hex.ToString("X"));
            sb.Append(delimiter);
        }
        if (sb.Length > delimiter.Length)
        {
            sb.Length -= delimiter.Length;
        }
        return sb.ToString();
    }
    private static byte ConvertToByte(BitArray bits)
    {
        if (bits.Count != 8)
        {
            throw new ArgumentException("bits");
        }
        byte[] bytes = new byte[1];
        bits.CopyTo(bytes, 0);
        return bytes[0];
    }
}
