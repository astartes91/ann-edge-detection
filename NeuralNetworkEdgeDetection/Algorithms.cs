using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeatonResearchNeural.Feedforward;
using HeatonResearchNeural.Feedforward.Train;
using HeatonResearchNeural.Feedforward.Train.Backpropagation;
using HeatonResearchNeural.Matrix;

namespace NeuralNetworkEdgeDetection
{
    class Algorithms
    {
        public static Dictionary<int, int> GetHistogram(Bitmap bitmap)
        {
            Dictionary<int, int> histogram = new Dictionary<int, int>();

            for (int i = 0; i < 256; i++)
            {
                histogram.Add(i, 0);
            }

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color originalColor = bitmap.GetPixel(i, j);
                    byte b = originalColor.R;
                    histogram[b]++;
                }
            }

            return histogram;
        }

        public static int GetOtsuTreshold(Dictionary<int, int> histogram, long total)
        {
            int sum = 0;
            for (int i = 1; i < 256; ++i)
            {
                sum += i * histogram[i];
            }

            int sumB = 0;
            int wB = 0;
            double max = 0.0;
            int threshold1 = 0;
            int threshold2 = 0;
            for (int i = 0; i < 256; ++i)
            {
                wB += histogram[i];
                if (wB == 0)
                {
                    continue;
                }

                long wF = total - wB;
                if (wF == 0)
                {
                    break;
                }

                sumB += i * histogram[i];
                double mB = (double)sumB / wB;
                double mF = (double)(sum - sumB) / wF;
                double between = wB * wF * Math.Pow(mB - mF, 2);
                if (between >= max)
                {
                    threshold1 = i;
                    if (between > max)
                    {
                        threshold2 = i;
                    }
                    max = between;
                }
            }
            return (threshold1 + threshold2) / 2;
            /*var sum = 0;
            for (var i = 1; i < 256; ++i)
                sum += i * histogram[i];
            var sumB = 0;
            var wB = 0;
            long wF = 0;
            double mB;
            double mF;
            double max = 0;
            double between;
            var threshold = 0;
            for (var i = 0; i < 256; ++i)
            {
                wB += histogram[i];
                if (wB == 0)
                    continue;
                wF = total - wB;
                if (wF == 0)
                    break;
                sumB += i * histogram[i];
                mB = sumB / wB;
                mF = (sum - sumB) / wF;
                between = wB * wF * Math.Pow(mB - mF, 2);
                if (between > max)
                {
                    max = between;
                    threshold = i;
                }
            }

            return threshold;*/
        }

        public static FeedforwardNetwork TrainANN(BackgroundWorker worker)
        {
            double[][] input =
            {
                new double[]{0,0,0,0},
                new double[]{0,0,0,1},
                new double[]{0,0,1,0},
                new double[]{0,0,1,1},
                new double[]{0,1,0,0},
                new double[]{0,1,0,1},
                new double[]{0,1,1,0},
                new double[]{0,1,1,1},
                new double[]{1,0,0,0},
                new double[]{1,0,0,1},
                new double[]{1,0,1,0},
                new double[]{1,0,1,1},
                new double[]{1,1,0,0},
                new double[]{1,1,0,1},
                new double[]{1,1,1,0},
                new double[]{1,1,1,1}                                           
            };

            double[][] output = 
            {                                              
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
                new double[]{1,1,1,1} 
            };

            FeedforwardNetwork network = new FeedforwardNetwork();
            network.AddLayer(new FeedforwardLayer(4));
            network.AddLayer(new FeedforwardLayer(12));
            network.AddLayer(new FeedforwardLayer(4));
            network.Reset();

            // train the neural network
            Train train = new Backpropagation(network, input, output,
                    0.2, 0.9);

            int epoch = 1;
            do
            {
                train.Iteration();
                worker.ReportProgress(0, "Epoch #" + epoch + " Error: " + train.Error);
                epoch++;
            } while ((epoch < 40000) && (train.Error > 0.01));

            using (StreamWriter file = new StreamWriter("weights.txt"))
            {
                FeedforwardLayer inputLayer = network.InputLayer;
                Matrix layerMatrix = inputLayer.LayerMatrix;
                for (int i = 0; i < layerMatrix.Rows; i++)
                {
                    for (int j = 0; j < layerMatrix.Cols; j++)
                    {
                        file.Write(layerMatrix[i, j]);
                        if (j != layerMatrix.Cols - 1)
                        {
                            file.Write(" ");
                        }
                    }
                    file.WriteLine();
                }

                file.WriteLine();

                FeedforwardLayer hiddenLayer = network.HiddenLayers.ToList()[0];
                layerMatrix = hiddenLayer.LayerMatrix;
                for (int i = 0; i < layerMatrix.Rows; i++)
                {
                    for (int j = 0; j < layerMatrix.Cols; j++)
                    {
                        file.Write(layerMatrix[i, j]);
                        if (j != layerMatrix.Cols - 1)
                        {
                            file.Write(" ");
                        }
                    }
                    file.WriteLine();
                }
            }

            return network;
        }

        /*public static void GetANN(BackgroundWorker worker)
        {
            /*List<String> lines = File.ReadAllLines("weights.txt").ToList();
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

            HeatonResearchNeural.Matrix.Matrix inputLayerMatrix = new HeatonResearchNeural.Matrix.Matrix(inputLayerWeights);
            HeatonResearchNeural.Matrix.Matrix hiddenLayermatrix = new HeatonResearchNeural.Matrix.Matrix(hiddenLayerWeights);*/

           /* FeedforwardNetwork network = new FeedforwardNetwork();
            network.AddLayer(new FeedforwardLayer(4));
            network.AddLayer(new FeedforwardLayer(12));
            network.AddLayer(new FeedforwardLayer(4));
            network.Reset();

            /*FeedforwardLayer inputLayer = network.InputLayer;
            inputLayer.LayerMatrix = inputLayerMatrix;

            FeedforwardLayer hiddenLayer = network.HiddenLayers.ToList()[0];
            hiddenLayer.LayerMatrix = hiddenLayermatrix;*/

            /*return network;
        }*/
    }
}
