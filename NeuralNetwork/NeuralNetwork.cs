using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// Back propagation structure
    /// </summary>
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
        private float n = 3.0f;
        private T label;
        private int expectedIndex = -1;
        /// <summary>
        /// A threshold for determining output validity
        /// </summary>
        private const float threshold = 0.5f;

        private Random random;

        public int[] Layers { get => layers; set => layers = value; }
        public float[][][] Weights { get => weights; set => weights = value; }
        public float[][] Biases { get => biases; set => biases = value; }

        /// <summary>
        /// Constructor for Neural Network
        /// </summary>
        /// <param name="layers">Number of neurons in each layer</param>
        public NeuralNetwork(int[] layers, T[] interpretations)
        {
            this.Layers = new int[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                this.Layers[i] = layers[i];
            }

            random = new Random(System.DateTime.Today.Millisecond);

            InitNeurons();
            InitWeights();
            InitBiases();
            interpret(interpretations);
        }

        public NeuralNetwork(int[] layers)
        {
            this.Layers = new int[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                this.Layers[i] = layers[i];
            }
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
            neurons = new float[Layers.Length][];
            for (int i = 0; i < Layers.Length; i++)
            {
                neurons[i] = new float[Layers[i]];
                for (int j = 0; j < Layers[i]; j++)
                {
                    neurons[i][j] = 0.0f;
                }
            }
            cost = new float[Layers[Layers.Length - 1]];
        }

        public void resetCost()
        {
            cost = new float[Layers[Layers.Length - 1]];
            costExist = false;
        }

        /// <summary>
        /// Initializes weights with a random value into weights array
        /// </summary>
        private void InitWeights()
        {
            //position of layers (4)
            Weights = new float[Layers.Length - 1][][];
            for (int i = 1; i < Weights.Length + 1; i++)
            {
                //position of the neuron (16)
                Weights[i - 1] = new float[neurons[i].Length][];
                for (int j = 0; j < Weights[i - 1].Length; j++)
                {
                    //position of the weight (784)
                    Weights[i - 1][j] = new float[neurons[i - 1].Length];
                    for (int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        float randNum = (float)random.NextDouble() - 0.5f;
                        Weights[i - 1][j][k] = randNum;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes biases with a random value into biases array
        /// </summary>
        private void InitBiases()
        {
            Biases = new float[Layers.Length - 1][];
            for (int i = 0; i < Biases.Length; i++)
            {
                Biases[i] = new float[neurons[i + 1].Length];
                for (int j = 0; j < Biases[i].Length; j++)
                {
                    float randNum = (float)random.NextDouble();
                    Biases[i][j] = randNum;
                }
            }
        }
        
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

            for (int i = 0; i < Layers.Length - 1; i++)
            {
                // next layer size
                for (int n = 0; n < Weights[i].Length; n++)
                {
                    // weights of each neuron in this layer
                    for (int w = 0; w < Weights[i][n].Length; w++)
                    {
                        sum += neurons[i][w] * Weights[i][n][w];
                    }
                    sum += Biases[i][n];
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
            int lastLayer = Layers.Length - 1;
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
            for (int i = Layers.Length - 2; i > 0; i--)
            {
                BackPropHidden(i);
            }
        }

        public void calcdCda()
        {
            int L = Layers.Length - 1;
            // for every neuron, i, in the last layer
            for (int i = 0; i < Layers[L]; i++)
            {
                dCda[L - 1][i] = 2.0f * (float)Math.Sqrt(cost[i]);
            }
        }

        public void BackPropInit(int batchSize)
        {
            float[][][] deltW = new float[Weights.Length][][];
            for (int i = 0; i < deltW.Length; i++)
            {
                deltW[i] = new float[Weights[i].Length][];
                for (int j = 0; j < deltW[i].Length; j++)
                {
                    deltW[i][j] = new float[Weights[i][j].Length];
                }
            }
            float[][] deltB = new float[Biases.Length][];
            for (int i = 0; i < deltB.Length; i++)
            {
                deltB[i] = new float[Biases[i].Length];
            }
            backPropstruct = new BackPropStruct(deltW, deltB, batchSize);
        }

        /// <summary>
        /// Back propagation for the output layer
        /// </summary>
        private void BackPropOutput()
        {
            int L = Layers.Length - 1;
            // for every neuron, i, in the last layer
            for (int i = 0; i < Layers[L]; i++)
            {
                int y = 0;
                // if the neuron index is the expectedIndex
                if (i == expectedIndex)
                {
                    y = 1;
                }
                dCda[L - 1][i] = (neurons[L][i] - y);
                // for every weights of the last layer
                for (int w = 0; w < Weights[L - 1][i].Length; w++)
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
            for (int i = 0; i < Layers[hL]; i++)
            {
                for (int curdCda = 0; curdCda < dCda[hL].Length; curdCda++)
                {
                    // dCda[0][0] +=   neurons[1][0...10] * dsig( weight[1] * neurons[1][0] + b[1][0..10]

                    dCda[hL - 1][i] += Weights[hL][curdCda][i] * derivativeSigmoid(Logit(neurons[hL + 1][curdCda])) * dCda[hL][curdCda];
                }
                //dCda[hL - 1][i] /= dCda[hL].Length;
                // we have previous layer's dCda[hL]
                // for every weight to be adjusted
                for (int w = 0; w < Weights[hL - 1][i].Length; w++)
                {
                    backPropstruct.deltW[hL - 1][i][w] += neurons[hL - 1][w] * derivativeSigmoid(Logit(neurons[hL][i])) * dCda[hL - 1][i];
                }
                for (int j = 0; j < Weights[hL].Length; j++)
                {
                    backPropstruct.deltB[hL - 1][i] += derivativeSigmoid(Logit(neurons[hL][i])) * dCda[hL - 1][i];
                }
            }
        }

        public void BackPropApplication()
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    for (int k = 0; k < Weights[i][j].Length; k++)
                    {
                        Weights[i][j][k] -= n * backPropstruct.deltW[i][j][k] / (float)backPropstruct.batchSize;
                    }
                }
            }
            for (int i = 0; i < Biases.Length; i++)
            {
                for (int j = 0; j < Biases[i].Length; j++)
                {
                    Biases[i][j] -= n * backPropstruct.deltB[i][j] / (float)backPropstruct.batchSize;
                }
            }
        }

        /// <summary>
        /// Initialize dCda array
        /// </summary>
        public void InitdCda()
        {
            dCda = new float[Weights.Length][];
            for (int i = 0; i < dCda.Length; i++)
            {
                dCda[i] = new float[Weights[i].Length];
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
            for (int i = 0; i < neurons[Layers.Length - 1].Length; i++)
            {
                float curMax = 0;
                if (neurons[Layers.Length - 1][i] >= threshold &&
                    neurons[Layers.Length - 1][i] > curMax)
                {
                    result = interpretation[i];
                    curMax = neurons[Layers.Length - 1][i];
                }
            }
            return result;
        }

        public void testWB(ref float[][][] weights, ref float[][] biases)
        {
            this.Weights = weights;
            this.Biases = biases;
        }
    }
}
