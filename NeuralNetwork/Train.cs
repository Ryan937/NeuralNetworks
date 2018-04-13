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
            float[][] data, T[] labels, int epochs, int batchSize)
        {
            for (int e = 0; e < epochs; e++)
            {
                nn.BackPropInit(batchSize);
                DataStruct dataS = ArrayTransform.Shuffle<T>(data, labels);
                for (int batchIndex = 0; batchIndex < batchSize; batchIndex++)
                {
                    nn.InitdCda();
                    nn.getInputs(dataS.data[batchIndex], dataS.labels[batchIndex]);
                    nn.calcOutput();
                    nn.calculateCost(dataS.labels[batchIndex]);
                    nn.calcGradientVector();
                    //nn.resetCost();
                }
                nn.BackPropApplication();
                trainResult(nn, dataS.data, dataS.labels, e, batchSize);
//                nn.resetCost();
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
            float[][] data, T[] labels, int curEpoch, int batchSize)
        {
            int success = 0;
            for (int batchIndex = 0; batchIndex < batchSize; batchIndex++)
            {
                if (labels[batchIndex].Equals(nn.determine(data[batchIndex])))
                {
                    success++;
                }
            }
            // edge case to be implemented
            Console.WriteLine("Epoch " + curEpoch + ": " + success + " / " + batchSize);
        }
    }
}
