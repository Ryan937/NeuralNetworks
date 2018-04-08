namespace NeuralNetwork
{
    class ArrayTransform
    {
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
    }
}
