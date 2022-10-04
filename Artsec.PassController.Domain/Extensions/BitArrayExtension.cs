using System.Collections;

namespace Artsec.PassController.Domain.Extensions;

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
