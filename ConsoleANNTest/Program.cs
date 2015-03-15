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
            List<String> lines = File.ReadAllLines("weights.txt").ToList();
            double[,] inputLayerWeights = new double[5, 12];
            double[,] hiddenLayerWeights = new double[13, 4];
            bool forInput = true;

            int counter = 0;
            foreach (string line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    forInput = false;
                    counter = 0;
                }
                else
                {
                    if (forInput)
                    {
                        string[] strings = line.Split(' ');
                        for (int i = 0; i < strings.Count(); i++)
                        {
                            inputLayerWeights[counter, i] = double.Parse(strings[i]);
                        }
                    }
                    else
                    {
                        string[] strings = line.Split(' ');
                        for (int i = 0; i < strings.Count(); i++)
                        {
                            hiddenLayerWeights[counter, i] = double.Parse(strings[i]);
                        }
                    }
                    counter++;
                }
            }

            

            Matrix inputLayerMatrix = new Matrix(inputLayerWeights);
            Matrix hiddenLayermatrix = new Matrix(hiddenLayerWeights);

            FeedforwardNetwork network = new FeedforwardNetwork();
            network.AddLayer(new FeedforwardLayer(4));
            network.AddLayer(new FeedforwardLayer(12));
            network.AddLayer(new FeedforwardLayer(4));
            network.Reset();

            FeedforwardLayer inputLayer = network.InputLayer;
            inputLayer.LayerMatrix = inputLayerMatrix;

            FeedforwardLayer hiddenLayer = network.HiddenLayers.ToList()[0];
            hiddenLayer.LayerMatrix = hiddenLayermatrix;
            
            Console.WriteLine("Neural Network Results:");
            for (int i = 0; i < XOR_IDEAL.Length; i++)
            {
                double[] actual = network.ComputeOutputs(XOR_INPUT[i]);
                Console.WriteLine(XOR_INPUT[i][0] + "," + XOR_INPUT[i][1] + "," + XOR_INPUT[i][2] + "," + XOR_INPUT[i][3]
                        + ", actual=" + actual[0] + "," + actual[1] + "," + actual[2] + "," + actual[3] + ",ideal=" + XOR_IDEAL[i][0] + "," + XOR_IDEAL[i][1] + "," + XOR_IDEAL[i][2] + "," + XOR_IDEAL[i][3]);
            }
            Console.ReadKey();

            /*Console.WriteLine("Input layer matrix:");
            Matrix layerMatrix = inputLayer.LayerMatrix;

            for (int i = 0; i < layerMatrix.Rows; i++)
            {
                for (int j = 0; j < layerMatrix.Cols; j++)
                {
                    Console.Write(Math.Round(layerMatrix[i, j], 3) + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Hidden layer matrix:");
            layerMatrix = hiddenLayer.LayerMatrix;
            for (int i = 0; i < layerMatrix.Rows; i++)
            {
                for (int j = 0; j < layerMatrix.Cols; j++)
                {
                    Console.Write(Math.Round(layerMatrix[i, j], 3) + " ");
                }
                Console.WriteLine();
            }  */
        }
    }
}
