using System;
using System.IO;

namespace NeuralNetwork
{
    /// <summary>
    /// Handle little and high endian parse
    /// </summary>
    /// <param name="br">Reader</param>
    /// <returns>Converted data</returns>
    static class BigEndianUtils
    {
        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
