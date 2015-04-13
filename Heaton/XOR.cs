using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using HeatonResearchNeural.Feedforward;
using HeatonResearchNeural.Feedforward.Train;
using HeatonResearchNeural.Matrix;

namespace Chapter5XOR
{
    /// <summary>
    /// Chapter 5: The Feedforward Backpropagation Neural Network
    /// 
    /// Solve the classic XOR problem.
    /// </summary>
    class XOR
    {
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT ={
            new double[] {0,0,0,0},
                new double[]{0,0,0,1},
                new double[]{0,0,1,0},
                new double[]{0,0,1,1},
                new double[]{0,1,0,0},
                new double[]{0,1,0,1},
                new double[]{0,1,1,0},
                new double[]{0,1,1,1},
                new double[]{1,0,0,0},
                new double[]{1,0,0,1},
               new double[] {1,0,1,0},
                new double[]{1,0,1,1},
                new double[]{1,1,0,0},
                new double[]{1,1,0,1},
                new double[]{1,1,1,0},
                new double[]{1,1,1,1}
                                            };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {                                              
            new double[]{1,1,1,1},
                new double[]{1,0,0,1},
                new double[]{0,1,1,0},
                new double[]{0,0,1,1},
                new double[]{0,1,1,0},
                new double[]{0,1,0,1},
                new double[]{0,1,1,0},
                new double[]{1,1,1,1},
                new double[]{1,0,0,1},
                new double[]{1,0,0,1},
                new double[]{1,0,1,0},
                new double[]{1,1,1,1},
                new double[]{1,1,0,0},
                new double[]{1,1,1,1},
                new double[]{1,1,1,1},
                new double[]{1,1,1,1} };

        /// <summary>
        /// Create, train and use a neural network for XOR.
        /// </summary>
        /// <param name="args">Not used</param>
        static void Main(string[] args)
        {
            FeedforwardNetwork network = new FeedforwardNetwork();
            network.AddLayer(new FeedforwardLayer(4));
            network.AddLayer(new FeedforwardLayer(12));
            network.AddLayer(new FeedforwardLayer(4));
            network.Reset();

            // train the neural network
            Train train = new HeatonResearchNeural.Feedforward.Train.Backpropagation.Backpropagation(network, XOR_INPUT, XOR_IDEAL,
                    0.5, 0.9);

            int epoch = 1;

            do
            {
                train.Iteration();
#if DEBUG
                Console.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
#endif
                epoch++;
            } while ((epoch < 40000) && (train.Error > 0.001));

            // test the neural network
#if DEBUG
            Console.WriteLine("Neural Network Results:");
            for (int i = 0; i < XOR_IDEAL.Length; i++)
            {
                double[] actual = network.ComputeOutputs(XOR_INPUT[i]);
                Console.WriteLine(XOR_INPUT[i][0] + "," + XOR_INPUT[i][1] + "," + XOR_INPUT[i][2] + "," + XOR_INPUT[i][3]
                        + ", actual=" + actual[0] + "," + actual[1] + "," + actual[2] + "," + actual[3] + ",ideal=" + XOR_IDEAL[i][0] + "," + XOR_IDEAL[i][1] + "," + XOR_IDEAL[i][2] + "," + XOR_IDEAL[i][3]);
            }
#endif

            FeedforwardLayer inputLayer = network.InputLayer;
            Console.WriteLine("Input layer matrix:");
            Matrix layerMatrix = inputLayer.LayerMatrix;

            using (StreamWriter file = new StreamWriter("weights.txt"))
            {
                for (int i = 0; i < layerMatrix.Rows; i++)
                {
                    for (int j = 0; j < layerMatrix.Cols; j++)
                    {
                        Console.Write(Math.Round(layerMatrix[i, j], 3) + " ");
                        file.Write(layerMatrix[i, j]);
                        if (j != layerMatrix.Cols - 1)
                        {
                            file.Write(" ");
                        }
                    }
                    Console.WriteLine();
                    file.WriteLine();
                }

                file.WriteLine();

                FeedforwardLayer hiddenLayer = network.HiddenLayers.ToList()[0];
                Console.WriteLine("Hidden layer matrix:");
                layerMatrix = hiddenLayer.LayerMatrix;
                for (int i = 0; i < layerMatrix.Rows; i++)
                {
                    for (int j = 0; j < layerMatrix.Cols; j++)
                    {
                        Console.Write(Math.Round(layerMatrix[i, j], 3) + " ");
                        file.Write(layerMatrix[i, j]);
                        if (j != layerMatrix.Cols - 1)
                        {
                            file.Write(" ");
                        }
                    }
                    Console.WriteLine();
                    file.WriteLine();
                }
            }

#if SHOW_MATRIX
            FeedforwardLayer outputLayer = network.OutputLayer;
            Console.WriteLine("Output layer matrix:");
            layerMatrix = outputLayer.LayerMatrix;
            for (int i = 0; i < layerMatrix.Rows; i++)
            {
                for (int j = 0; j < layerMatrix.Cols; j++)
                {
                    Console.Write(layerMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }

            foreach (FeedforwardLayer feedforwardLayer in network.Layers)
            {
                Matrix layerMatrix = feedforwardLayer.LayerMatrix;
                Console.WriteLine(feedforwardLayer.Next);
                for (int i = 0; i < layerMatrix.Rows; i++)
                {
                    for (int j = 0; j < layerMatrix.Cols; j++)
                    {
                        Console.Write(layerMatrix[i, j] + " ");
                    }
                    Console.WriteLine();
                }                
            }
#endif

            Console.ReadKey();
        }
    }
}
