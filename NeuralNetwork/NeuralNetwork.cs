using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    struct BackPropStruct
    {
        public float[][][] deltW;
        public float[][] deltB;
        public int batchSize;

        public BackPropStruct(float[][][] deltW, float[][] deltB, int batchSize)
        {
            this.deltW = deltW;
            this.deltB = deltB;
            this.batchSize = batchSize;
        }
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
        private float[][] dCda;
        private float[] cost;
        private bool costExist = false;
        //private bool gradVectExist = false;
        BackPropStruct backPropstruct;
        private T[] interpretation;
        private float n = 1.0f;
        private T label;
        private int expectedIndex = -1;
        /// <summary>
        /// A threshold for determining output validity
        /// </summary>
        private const float threshold = 0.5f;

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

        public void resetCost()
        {
            cost = new float[layers[layers.Length - 1]];
            costExist = false;
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
        /// Get inpu without label attached, for determining uses mostly
        /// </summary>
        /// <param name="inputs">Input array</param>
        public void getInputs(float[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                // always storing to first layer since it is an input
                neurons[0][i] = inputs[i];
            }
        }


        // Checked
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
                    sum = 0;
                }
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
            BackPropOutput();
            for (int i = layers.Length - 2; i > 0; i--)
            {
                BackPropHidden(i);
            }
        }

        public void calcdCda()
        {
            int L = layers.Length - 1;
            // for every neuron, i, in the last layer
            for (int i = 0; i < layers[L]; i++)
            {
                dCda[L - 1][i] = 2.0f * (float)Math.Sqrt(cost[i]);
            }
        }

        public void BackPropInit(int batchSize)
        {
            float[][][] deltW = new float[weights.Length][][];
            for (int i = 0; i < deltW.Length; i++)
            {
                deltW[i] = new float[weights[i].Length][];
                for (int j = 0; j < deltW[i].Length; j++)
                {
                    deltW[i][j] = new float[weights[i][j].Length];
                }
            }
            float[][] deltB = new float[biases.Length][];
            for (int i = 0; i < deltB.Length; i++)
            {
                deltB[i] = new float[biases[i].Length];
            }
            backPropstruct = new BackPropStruct(deltW, deltB, batchSize);
        }

        /// <summary>
        /// Back propagation for the output layer
        /// </summary>
        private void BackPropOutput()
        {
            int L = layers.Length - 1;
            // for every neuron, i, in the last layer
            for (int i = 0; i < layers[L]; i++)
            {
                int y = 0;
                // if the neuron index is the expectedIndex
                if (i == expectedIndex)
                {
                    y = 1;
                }
                dCda[L - 1][i] = 2.0f * (neurons[L][i] - y);
                // for every weights of the last layer
                for (int w = 0; w < weights[L - 1][i].Length; w++)
                {
                    // calculate and store dCda
                    // adjust weights using learning rate and dCdw
                    backPropstruct.deltW[L - 1][i][w] += neurons[L - 1][w] * derivativeSigmoid(Logit(neurons[L][i])) * dCda[L - 1][i];
                }
                backPropstruct.deltB[L - 1][i] += derivativeSigmoid(Logit(neurons[L][i])) * dCda[L - 1][i];
            }
        }

        private float Logit(float z)
        {
            return (float)Math.Log(z / (1 - z));
        }

        /// <summary>
        /// Back propagation on given hidden layer
        /// </summary>
        /// <param name="hL">hidden layer with respect to number of layers - 1</param>
        private void BackPropHidden(int hL)
        {
            // hL is the hidden layer with respect to numbeer of layers - 1
            // for every neuron in this hidden layer
            for (int i = 0; i < layers[hL]; i++)
            {
                for (int curdCda = 0; curdCda < dCda[hL].Length; curdCda++)
                {
                    // dCda[0][0] +=   neurons[1][0...10] * dsig( weight[1] * neurons[1][0] + b[1][0..10]

                    dCda[hL - 1][i] += weights[hL][curdCda][i] * derivativeSigmoid(Logit(neurons[hL + 1][curdCda])) * dCda[hL][curdCda];
                }
                // we have previous layer's dCda[hL]
                // for every weight to be adjusted
                for (int w = 0; w < weights[hL - 1][i].Length; w++)
                {
                    backPropstruct.deltW[hL - 1][i][w] += neurons[hL - 1][w] * derivativeSigmoid(Logit(neurons[hL][i])) * dCda[hL - 1][i];
                }
                for (int j = 0; j < weights[hL].Length; j++)
                {
                    backPropstruct.deltB[hL - 1][i] += weights[hL][j][i] * derivativeSigmoid(Logit(neurons[hL][i])) * dCda[hL - 1][i];
                }
            }
        }

        public void BackPropApplication()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] -= n * backPropstruct.deltW[i][j][k] / (float)backPropstruct.batchSize;
                    }
                }
            }
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] -= n * backPropstruct.deltB[i][j] / (float)backPropstruct.batchSize;
                }
            }
        }

        /// <summary>
        /// Initialize dCda array
        /// </summary>
        public void InitdCda()
        {
            dCda = new float[weights.Length][];
            for (int i = 0; i < dCda.Length; i++)
            {
                dCda[i] = new float[weights[i].Length];
            }
        }

        //private void addGradVectToWeight()
        //{
        //    int index = 0;

        //    for (int i = layers.Length - 1; i > 0; i--)
        //    {
        //        for (int j = 0; j < neurons[i].Length; j++)
        //        {
        //            for (int k = 0; k < weights[i][j].Length; k++)
        //            {
        //                weights[i][j][k] += -n * gradientVector.weights[index++];
        //            }
        //        }
        //    }
        //}

        //private void addGradVectToBias()
        //{
        //    int index = 0;

        //    for (int i = layers.Length - 1; i > 0; i--)
        //    {
        //        for (int j = 0; j < neurons[i].Length; j++)
        //        {
        //            biases[i][j] += -n * gradientVector.biases[index++];
        //        }
        //    }
        //}

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

        public void testWB(ref float[][][] weights, ref float[][] biases)
        {
            this.weights = weights;
            this.biases = biases;
        }
    }
}
