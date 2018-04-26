using System;

namespace NeuralNetwork
{
    class ArrayTransform
    {
        static Random _random = new Random();
        /// <summary>
        /// Returns byte after checking for boundary
        /// </summary>
        /// <param name="tempValue">Value to be casted into byte</param>
        /// <returns></returns>
        public static byte getBoundedByte(double tempValue)
        {
            if (tempValue < 0)
            {
                tempValue = 0;
            }
            else if (tempValue > 255)
            {
                tempValue = 255;
            }

            return (byte)tempValue;
        }

        /// <summary>
        /// Shuffle two d array.
        /// modified version of source below
        /// Source: https://forums.asp.net/t/1778021.aspx?shuffle+int+array+in+c+
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public static Train<T>.DataStruct Shuffle<T>(double[][] array, T[] labels)
        {
            var random = _random;
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                                        // Swap.
                double[] tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
                T tmpl = labels[j];
                labels[j] = labels[i - 1];
                labels[i - 1] = tmpl;
            }
            Train<T>.DataStruct data = new Train<T>.DataStruct(array, labels);
            return data;
        }
    }
}
