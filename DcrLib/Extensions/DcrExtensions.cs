using System.Text;

namespace DcrNet.Extensions;

public static class DcrExtensions
{
    /// <summary>
    ///     Searches for bytes pattern in specified binary stream and returns start position
    /// </summary>
    /// <param name="reader">reader for file stream</param>
    /// <param name="pattern">searched pattern</param>
    /// <param name="bufferSize">size of single search step</param>
    /// <returns>position for first pattern match</returns>
    public static long FindPatternPosition(this BinaryReader reader, byte[] pattern, int bufferSize)
    {
        long position = -1;
        long diff = 0;
        while (position < 0)
        {
            var buffer = reader.ReadBytes(bufferSize);
            position = buffer.FindPatternPosition(pattern, 0);

            diff = buffer.LongLength - position;
        }

        return reader.BaseStream.Position - diff;
    }

    /// <summary>
    ///     Similar to <see cref="FindPatternPosition(System.IO.BinaryReader,byte[],int)"/>, but used for bytes array instead of stream and didn't use bytes buffer
    /// </summary>
    /// <param name="data">bytes array for search</param>
    /// <param name="pattern">searched pattern</param>
    /// <param name="startIndex">zero-based index, from which search starts</param>
    /// <returns>first match position</returns>
    public static long FindPatternPosition(this byte[] data, byte[] pattern, long startIndex)
    {
        for (var i = startIndex; i <= data.Length - pattern.Length; i++)
        {
            var found = !pattern.Where((t, j) => data[i + j] != t).Any();

            if (found) return i;
        }

        return -1;
    }

    /// <summary>
    ///     Converts specified range of bytes into ASCII string
    /// </summary>
    /// <param name="data">bytes array with desired value</param>
    /// <param name="pos">offset for array</param>
    /// <param name="length">size of desired value</param>
    /// <returns>ASCII-encoded string</returns>
    public static string ParseString(this byte[] data, ref int pos, int length)
    {
        var output = new byte[length];
        Array.Copy(data, pos, output, 0, length);
        pos += length;
        return Encoding.ASCII.GetString(output);
    }

    /// <summary>
    ///     Converts specified range of bytes into signed integer
    /// </summary>
    /// <param name="data">bytes array with desired value</param>
    /// <param name="pos">offset for array</param>
    /// <param name="length">size of desired value</param>
    /// <returns>32-bit signed integer</returns>
    public static int ParseInt(this byte[] data, ref int pos, int length)
    {
        var output = new byte[length];
        Array.Copy(data, pos, output, 0, length);
        pos += length;
        return BitConverter.ToInt32(output, 0);
    }

    /// <summary>
    ///     Converts specified range of bytes into signed short
    /// </summary>
    /// <param name="data">bytes array with desired value</param>
    /// <param name="pos">offset for array</param>
    /// <param name="length">size of desired value</param>
    /// <returns>16-bit signed short</returns>
    public static short ParseShort(this byte[] data, ref int pos, int length)
    {
        var output = new byte[2];
        Array.Copy(data, pos, output, 0, length);
        pos += length;
        return BitConverter.ToInt16(output, 0);
    }

    /// <summary>
    ///     Extracts array from specified bytes sequence
    /// </summary>
    /// <param name="data">bytes array with desired value</param>
    /// <param name="pos">offset for array</param>
    /// <param name="length">size of desired value</param>
    /// <returns>bytes array</returns>
    public static byte[] ParseSubArray(this byte[] data, ref int pos, int length)
    {
        var output = new byte[length];
        Array.Copy(data, pos, output, 0, length);
        var lastIndex = Array.FindLastIndex(output, b => b != 0);
        Array.Resize(ref output, lastIndex + 1);
        pos += length;
        return output;
    }

    /// <summary>
    ///     Checks if searched pattern starts from specified position
    /// </summary>
    /// <param name="fileBytes">bytes array</param>
    /// <param name="pattern">searched pattern</param>
    /// <param name="position">zero-based index, from which search starts</param>
    /// <returns></returns>
    public static bool StartsWithAt(this byte[] fileBytes, byte[] pattern, long position)
    {
        if (position + pattern.Length > fileBytes.Length)
            return false;

        for (var i = 0; i < pattern.Length; i++)
            if (fileBytes[position + i] != pattern[i])
                return false;
        return true;
    }
}