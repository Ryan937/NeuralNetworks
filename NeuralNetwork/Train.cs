using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class Train<T>
    {
        public struct DataStruct
        {
            public float[][] data;
            public T[] labels;

            public DataStruct(float[][] data, T[] labels)
            {
                this.data = data;
                this.labels = labels;
            }
        }

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
            DataStruct dataS = ArrayTransform.Shuffle<T>(data, labels);
            int batchSize = (int)Math.Ceiling((double)labels.Length / epochs);
            int curData = 0;
            int curBackProp = 0;
            for (int e = 0; e < epochs; e++)
            {
                for (int batchIndex = 0; batchIndex < batchSize &&
                    curData < labels.Length; batchIndex++, curData++)
                {
                    nn.getInputs(dataS.data[curData], dataS.labels[curData]);
                    nn.calcOutput();
                    nn.calculateCost(dataS.labels[curData]);
                }
                for (int batchIndex = 0; batchIndex < batchSize &&
                    curBackProp < labels.Length; batchIndex++, curBackProp++)
                {
                    nn.getInputs(dataS.data[curBackProp], dataS.labels[curBackProp]);
                    nn.calcOutput();
                    nn.calcGradientVector();
                }
                trainResult(nn, data, labels, epochs, e, curData - batchSize);
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
