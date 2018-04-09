using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class DigitNN
    {
        public const int IMAGE_SIDE = 28;

        public static void init(out NeuralNetwork<byte> nw, int[] layers)
        {
            byte[] meaning = new byte[10];
            for (int i = 0; i < meaning.Length; i++)
            {
                meaning[i] = (byte)i;
            }
            nw = new NeuralNetwork<byte>(layers, meaning);
        }

        /// <summary>
        /// Transform bitmap data into float array
        /// </summary>
        /// <param name="data">Input bitmap</param>
        /// <returns>Float format result</returns>
        public static float[][] transformBitmapdata(Bitmap[] data)
        {
            float[][] result = new float[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = new float[IMAGE_SIDE * IMAGE_SIDE];
                // data bitmap size check handling isn't implemented
                for (int h = 0, index = 0; h < IMAGE_SIDE; h++)
                {
                    for (int w = 0; w < IMAGE_SIDE; w++)
                    {
                        Color c = data[i].GetPixel(w, h);
                        result[i][index++] = (c.R + c.G + c.B) / 3.0f;
                    }
                }
            }
            return result;
        }
    }
}
