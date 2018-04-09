using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class NeuralNetwork
    {
        private int[] layers;
        /// <summary>
        /// Layers of neurons of the neural network
        /// </summary>
        private float[][] neurons;
        private float[][][] weights;
        private float[][] biases;
        private float[] cost;

        private Random random;

        /// <summary>
        /// Constructor for Neural Network
        /// </summary>
        /// <param name="layers">Number of neurons in each layer</param>
        public NeuralNetwork(int[] layers)
        {
            this.layers = new int[layers.Length];  
            for (int i = 0; i < layers.Length; i++)
            {
                this.layers[i] = layers[i];
            }

            random = new Random(System.DateTime.Today.Millisecond);

            InitNeurons();
            InitWeights();
            InitBiases();
        }

        /// <summary>
        /// Creates the right number of neurons for all layers
        /// </summary>
        private void InitNeurons()
        {
            neurons = new float[layers.Length][];
            for (int i = 0; i < layers.Length; i++)
            {
                neurons[i] = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                {
                    neurons[i][j] = 0.0f;
                }
            }
            cost = new float[layers[layers.Length - 1]];
        }

        /// <summary>
        /// Initializes weights with a random value into weights array
        /// </summary>
        private void InitWeights()
        {
            //position of layers (4)
            weights = new float[layers.Length - 1][][];
            for (int i = 1; i < weights.Length + 1; i++)
            {
                //position of the neuron (16)
                weights[i - 1] = new float[neurons[i].Length][];
                for (int j = 0; j < weights[i - 1].Length; j++)
                {
                    //position of the weight (784)
                    weights[i - 1][j] = new float[neurons[i-1].Length];
                    for (int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        float randNum = (float)random.NextDouble() - 0.5f;
                        weights[i - 1][j][k] = randNum;
                    }
                }
            }
            
            //Test for above triple for loop
            /*for (int i = 0; i < weights.Length - 1; i++)
            {
                Debug.WriteLine("-----------------");
                for (int j = 0; j < weights[i].Length - 1; j++)
                {
                    Debug.WriteLine("-----------------");
                    for (int k = 0; k < weights[i][j].Length - 1; k++)
                    {
                        Debug.Write(weights[i][j][k] + " ");
                    }
                }
            }*/
        }

        /// <summary>
        /// Initializes biases with a random value into biases array
        /// </summary>
        private void InitBiases()
        {
            biases = new float[layers.Length - 1][];
            for (int i = 0; i < biases.Length; i++)
            {
                biases[i] = new float[neurons[i + 1].Length];
                for (int j = 0; j < biases[i].Length; j++)
                {
                    float randNum = (float)random.NextDouble();
                    biases[i][j] = randNum;
                }
            }
        }
        
        /*private float randVariables()
        {
            float mean = 100;
            float stdDev = 10;
            Random rand = new Random(); //reuse this if you are generating many
            float u1 = (float)(1.0 - rand.NextDouble()); //uniform(0,1] random doubles
            float u2 = (float)(1.0 - rand.NextDouble());
            float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * 
                Math.Sin(2.0 * Math.PI * u2)); //random normal(0,1)
            float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }*/
        
        /// <summary>
        /// Sigmoid function to make the value of "a" to be somewhere 0 to 1 
        /// </summary>
        /// <param name="x">The "a"</param>
        /// <returns>Sigmoid of "a"</returns>
        private float Sigmoid(float x)
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }

        /// <summary>
        /// Each neurons take a part of the image and stores "a" into neurons array
        /// </summary>
        /// <param name="inputs">The float array of inputs</param>
        private void getInputs(float[][] inputs)
        {
            for (int i = 0, index = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                { 
                    // always storing to first layer since it is an input
                    neurons[0][index++] = inputs[i][j];
                }
            }
        }

        /// <summary>
        /// Calculates neurons * weights + biases to get 10 outputs.
        /// The outputs will be saved in neurons array.
        /// </summary>
        private void calcOutput()
        {
            float sum = 0;

            for (int i = 0; i < layers.Length - 1; i++)
            {
                // next layer size
                for (int n = 0; n < weights[i].Length; n++)
                {
                    // weights of each neuron in this layer
                    for (int w = 0; w < weights[i][n].Length; w++)
                    {
                        sum += neurons[i][w] * weights[i][n][w];
                    }
                    sum += biases[i][n];
                    sum = Sigmoid(sum);
                    neurons[i + 1][n] = sum;
                }

                //for (int j = 0; j < neurons[i].Length; j++)
                //{
                //    for (int k = 0; k < weights[i][j].Length; k++)
                //    {
                //        sum += neurons[i][j] * weights[i][j][k];
                //    }
                //    sum += biases[i][j];
                //    sum = Sigmoid(sum);
                //    // layer + 1 for storing result
                //    neurons[i + 1][j] = sum;
                //}
            }
        }

        private void calculateCost(byte value)
        {
            float costTemp = 0.0f;
            int lastLayer = layers.Length - 1;
            float expectedOutput = 0.0f;
            int v = Convert.ToInt32(value);

            //Loops at the output layer
            for (int i = 0; i < neurons[lastLayer].Length; i++)
            {
                if (i == v)
                {
                    expectedOutput = 1.00f;
                }

                costTemp += (float)Math.Pow((neurons[lastLayer][i] - expectedOutput), 2);

                if (expectedOutput == 1.00f)
                {
                    expectedOutput = 0.00f;
                }
            }
            cost[v] = (cost[v] + costTemp) / 2;
        }
    }
}
