using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    struct GradientVector
    {
        public float[] weights;
        public float[] biases;
    };
    
    class NeuralNetwork<T>
    {
        private int[] layers;
        /// <summary>
        /// Layers of neurons of the neural network
        /// </summary>
        private float[][] neurons;
        private float[][][] weights;
        private float[][] biases;
        private float[][][] dCda;
        private float[] cost;
        private bool costExist = false;
        private bool gradVectExist = false;
        GradientVector gradientVector;
        private T[] interpretation;
        private float n = 3;
        private T label;
        private int expectedIndex = -1;
        /// <summary>
        /// A threshold for determining output validity
        /// </summary>
        private const float threshold = 0.8f;

        private Random random;

        /// <summary>
        /// Constructor for Neural Network
        /// </summary>
        /// <param name="layers">Number of neurons in each layer</param>
        public NeuralNetwork(int[] layers, T[] interpretations)
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
            interpret(interpretations);
        }

        /// <summary>
        /// Setup interpretation for the neural network
        /// </summary>
        /// <param name="interpretations">Given interpretations</param>
        private void interpret(T[] interpretations)
        {
            interpretation = new T[interpretations.Length];
            for (int i = 0; i < interpretations.Length; i++)
            {
                interpretation[i] = interpretations[i];
            }
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
                    weights[i - 1][j] = new float[neurons[i - 1].Length];
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
        /// Derivative of the sigmoid function
        /// </summary>
        /// <param name="x">The "a"</param>
        /// <returns>Derivative of the sigmoid "a"</returns>
        private float derivativeSigmoid(float x)
        {
            return (float)(Math.Exp(-x) / Math.Pow((1 + Math.Exp(-x)), 2));
        }

        /// <summary>
        /// Each neurons take a part of the image and stores "a" into neurons array
        /// </summary>
        /// <param name="inputs">The float array of inputs</param>
        public void getInputs(float[] inputs, T label)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                // always storing to first layer since it is an input
                neurons[0][i] = inputs[i];
            }
            this.label = label;
            for (int i = 0; i < interpretation.Length; i++)
            {
                if (label.Equals(interpretation[i]))
                {
                    expectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Calculates neurons * weights + biases to get 10 outputs.
        /// The outputs will be saved in neurons array.
        /// </summary>
        public void calcOutput()
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

        /// <summary>
        /// Calculate the cost and add to the overall average cost
        /// </summary>
        /// <param name="value"></param>
        public void calculateCost(T value)
        {
            float costTemp = 0.0f;
            int lastLayer = layers.Length - 1;
            float expectedOutput = 0.0f;
            int v = 0;
            for (int i = 0; i < interpretation.Length; i++)
            {
                if (interpretation[i].Equals(value))
                {
                    v = i;
                    break;
                }
            }

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

            if (costExist == false)
            {
                costExist = true;
                cost[v] = costTemp;

            }
            else if (costExist == true)
            {
                cost[v] = (cost[v] + costTemp) / 2;
            }
        }

        public void calcGradientVector()
        {
            //neurons[L-1][] * derivativeSigmoid(neurons[L][]) * 2(cost[L])
            //
            //  I       H        O
            //  784    100       10
            int weightSize = 0;
            int biasesSize = 0;
            for (int i = 0; i < layers.Length - 1; i++)
            {
                weightSize += layers[i] * layers[i + 1];
            }
            gradientVector.weights = new float[weightSize];
            for (int i = 1; i < layers.Length; i++)
            {
                biasesSize += layers[i];
            }
            gradientVector.biases = new float[biasesSize];
            // last input
            // do for every neuron to all neuron
            //  I       H        O       Y
            //  -      100       0       0
            //  -      100       1       0
            //  -      100       2       1
            //  -      100       3       0
            //  -      100      ...      0
            //  -      100       10      0
            // 784  w   0        -
            // 784  w   1        -
            // 784  w  ...       -
            // 784  w  100       -
            for (int i = layers.Length - 1, index = 0; i > 0; i--)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    for (int w = 0; w < weights[i - 1][j].Length; w++)
                    {
                        dCda[i - 1][j][w] = calcdCda(i, j, w);
                        // cost size 10 (last layer's size)
                        // last layer       -    10 neurons
                        // last layer - 1   -    100 neurons 
                        gradientVector.weights[index++] = neurons[i - 1][j] *
                            derivativeSigmoid(neurons[i][j]) * 2 * Math.Sqrt(cost[;

                    }

                    gradientVector.biases[j] = derivativeSigmoid(neurons[i][j]) * 2 * (cost[i]);
                }
            }
            addGradVectToWeight();
            addGradVectToBias();
        }

        private float calcdCdW(int L, int n, int w)
        {
            //  L0   L1    L2    L3
            //  I    h1    h2    O
            //    w            
            //
            //
            float dCdw = 0.0f;
            // Starting from L2 with respect to weight layers
            // for every connection calculate and sum 
            
            dCda = new float[weights.Length][][];
            for (int i = layers.Length - 1; i > L; i++)
            {
                for (int mN = 0; mN < neurons[i].Length; mN++)
                {
                    for (int mW = 0; mW < weights[i - 1][mN].Length; mW++)
                    {
                        float dzdw = neurons[L - 1][n];
                        float dadz = derivativeSigmoid(calcZ(L, n, w));
                        dCdw += dzdw * dadz * dCda[i - 1][mN][mW];
                    }
                }
            }


            if (L == layers.Length - 1)
            {
                dCdw = weights[L - 1][n][w] * derivativeSigmoid(calcZ(L, n, w));
            }
            return 0.0f;
        }

        private float calcdCda(int L, int n, int w)
        {
            int y = 0;
            if (n == expectedIndex)
            {
                y = 1;
            }
            if (L == layers.Length - 1)
            {
                return 2.0f * (neurons[L][n] - y);
            }
            return 2.0f * weights[L][n][w] * derivativeSigmoid(calcZ(L, n, w) * (neurons[L][n] - y));
        }

        private float calcZ(int L, int n, int w)
        {
            float z = 0.0f;
            z = neurons[L - 1][n] * weights[L - 1][n][w] + biases[L - 1][n];
            return z;
        }

        private void addGradVectToWeight()
        {
            int index = 0;

            for (int i = layers.Length - 1; i > 0; i--)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] += -n * gradientVector.weights[index++];
                    }
                }
            }
        }

        private void addGradVectToBias()
        {
            int index = 0;

            for (int i = layers.Length - 1; i > 0; i--)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    biases[i][j] += -n * gradientVector.biases[index++];
                }
            }
        }

        /// <summary>
        /// Given an input, determine the output
        /// </summary>
        /// <param name="inputs">input data</param>
        /// <returns>Result index</returns>
        public T determine(float[] inputs)
        {
            T result = default(T);
            getInputs(inputs);
            calcOutput();
            for (int i = 0; i < neurons[layers.Length - 1].Length; i++)
            {
                float curMax = 0;
                if (neurons[layers.Length - 1][i] >= threshold &&
                    neurons[layers.Length - 1][i] > curMax)
                {
                    result = interpretation[i];
                    curMax = neurons[layers.Length - 1][i];
                }
            }
            return result;
        }
    }
}
