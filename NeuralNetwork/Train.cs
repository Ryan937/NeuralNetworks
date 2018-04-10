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
            float[][] data, T[] labels, int epochs)
        {
            int batchSize = (int)Math.Ceiling((double)labels.Length / epochs);
            int curData = 0;
            for (int e = 0; e < epochs; e++)
            {
                for (int batchIndex = 0; batchIndex < batchSize &&
                    curData < labels.Length; batchIndex++, curData++)
                {
                    nn.getInputs(data[curData]);
                    nn.calcOutput();
                    nn.calculateCost(labels[curData]);
                }
                nn.calcGradientVector();
                trainResult(nn, data, labels, epochs, e, curData);
            }
        }

        /// <summary>
        /// Print out epoch sessions
        /// </summary>
        /// <param name="nn">neural network</param>
        /// <param name="data">whole input data</param>
        /// <param name="labels">whole labels</param>
        /// <param name="epochs">number of epochs</param>
        /// <param name="curEpoch">current epoch</param>
        /// <param name="index">current data index</param>
        public static void trainResult(NeuralNetwork<T> nn,
            float[][] data, T[] labels, int epochs, int curEpoch, int index)
        {
            int batchSize = (int)Math.Ceiling((double)labels.Length / epochs);
            int curData = index;
            int success = 0;
            for (int batchIndex = 0; batchIndex < batchSize &&
                curData < labels.Length; batchIndex++, curData++)
            {
                if (labels[curData].Equals(nn.determine(data[curData])))
                {
                    success++;
                }
            }
            // edge case to be implemented
            Console.WriteLine("Epoch " + curEpoch + ": " + success + " / " + batchSize);
        }
    }
}
