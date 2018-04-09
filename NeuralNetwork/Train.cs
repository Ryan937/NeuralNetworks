using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class Train<T>
    {
        /// <summary>
        /// Train a network using given data, label, and batchSize
        /// </summary>
        /// <param name="nn">neural network</param>
        /// <param name="data">training data</param>
        /// <param name="labels">data's label</param>
        /// <param name="batchSize">Batch size</param>
        /// <returns></returns>
        public static void trainNetwork(NeuralNetwork<T> nn, 
            float[][] data, T[] labels, int batchSize)
        {
            int epochs = (int)Math.Ceiling((double)labels.Length / batchSize);
            int curData = 0;
            for (int e = 0; e < epochs; e++)
            {
                for (int batchIndex = 0; batchIndex < batchSize; batchIndex++, curData++)
                {
                    nn.getInputs(data[curData]);
                    nn.calcOutput();
                    nn.calculateCost(labels[curData]);
                }
            }
        }
    }
}
