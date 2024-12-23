using System.Text;

namespace DcrLib.Extensions;

public static class DcrExtensions
{
    public static long FindPatternPosition(this byte[] data, byte[] pattern, long startIndex)
    {
        for (long i = startIndex; i <= data.Length - pattern.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (data[i + j] != pattern[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
            {
                return i;
            }
        }
        return -1;
    }

    public static string ParseString(this byte[] data, ref int pos, int length)
    {
        byte[] output = new byte[length];
        Array.Copy(data, pos, output, 0, length);
        pos += length;
        return Encoding.ASCII.GetString(output);
    }

    public static int ParseInt(this byte[] data, ref int pos, int length)
    {
        byte[] output = new byte[length];
        Array.Copy(data, pos, output, 0, length);
        pos += length;
        return BitConverter.ToInt32(output, 0);
    }

    public static byte[] ParseSubArray(this byte[] data, ref int pos, int length)
    {
        byte[] output = new byte[length];
        Array.Copy(data, pos, output, 0, 20);
        int lastIndex = Array.FindLastIndex(output, b => b != 0);
        Array.Resize(ref output, lastIndex + 1);
        pos += length;
        return output;
    }
}