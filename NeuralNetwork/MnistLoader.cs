using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class MnistLoader
    {
        public static void LoadMnist(string path, string secondPath, out Bitmap[] bitmap, out byte[] labels)
        {
            bitmap = null;
            labels = null;
            try
            {
                Console.WriteLine("\nBegin\n");
                FileStream ifsLabels =
                 new FileStream(path,
                 FileMode.Open); // test labels
                FileStream ifsImages =
                 new FileStream(secondPath,
                 FileMode.Open); // test images

                BinaryReader brLabels =
                 new BinaryReader(ifsLabels);
                BinaryReader brImages =
                 new BinaryReader(ifsImages);

                int magic1 = BigEndianUtils.ReadBigInt32(brImages); // discard
                int numImages = BigEndianUtils.ReadBigInt32(brImages);
                int numRows = BigEndianUtils.ReadBigInt32(brImages);
                int numCols = BigEndianUtils.ReadBigInt32(brImages);
                bitmap = new Bitmap[numImages];

                int magic2 = BigEndianUtils.ReadBigInt32(brLabels);
                int numLabels = BigEndianUtils.ReadBigInt32(brLabels);
                labels = new byte[numLabels];

                // each test image
                for (int di = 0; di < 2; ++di)
                {
                    bitmap[di] = new Bitmap(numRows, numCols);
                    for (int i = 0; i < 28; ++i)
                    {
                        for (int j = 0; j < 28; ++j)
                        {
                            byte b = brImages.ReadByte();
                            bitmap[di].SetPixel(j, i, Color.FromArgb(b, b, b));
                        }
                    }
                    labels[di] = brLabels.ReadByte();
                } // each image

                ifsImages.Close();
                brImages.Close();
                ifsLabels.Close();
                brLabels.Close();

                Console.WriteLine("\nEnd\n");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
