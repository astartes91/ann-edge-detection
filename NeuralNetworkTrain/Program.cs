using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworkEdgeDetection;

namespace NeuralNetworkTrain
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] inputPatterns = new int[,]
            {
                {0,0,0,0},
                {0,0,0,1},
                {0,0,1,0},
                {0,0,1,1},
                {0,1,0,0},
                {0,1,0,1},
                {0,1,1,0},
                {0,1,1,1},
                {1,0,0,0},
                {1,0,0,1},
                {1,0,1,0},
                {1,0,1,1},
                {1,1,0,0},
                {1,1,0,1},
                {1,1,1,0},
                {1,1,1,1}
            };

            int[,] outputPatterns = new int[,]
            {
                {0,0,0,0},
                {1,0,0,1},
                {0,1,1,0},
                {0,0,1,1},
                {0,1,1,0},
                {0,1,0,1},
                {0,1,1,0},
                {0,0,0,0},
                {1,0,0,1},
                {1,0,0,1},
                {1,0,1,0},
                {0,0,0,0},
                {1,1,0,0},
                {0,0,0,0},
                {0,0,0,0},
                {0,0,0,0}
            };

            Random rand = new Random();

            /*List<Neuron> hiddenNeurons = new List<Neuron>();
            for (int i = 0; i < 12; i++)
            {
                Neuron hiddenNeuron = new Neuron();
                hiddenNeuron.Weights = new double[5];

                for (int j = 0; j < 5; j++)
                {
                    double randValue = rand.NextDouble();
                    while (randValue == 0)
                    {
                        randValue = rand.NextDouble();
                    }
                    hiddenNeuron.Weights[j] = randValue;
                }
            }

            List<Neuron> outputNeurons = new List<Neuron>();
            for (int i = 0; i < 4; i++)
            {
                Neuron hiddenNeuron = new Neuron();
                hiddenNeuron.Weights = new double[13];

                for (int j = 0; j < 13; j++)
                {
                    double randValue = rand.NextDouble();
                    while (randValue == 0)
                    {
                        randValue = rand.NextDouble();
                    }
                    hiddenNeuron.Weights[j] = randValue;
                }
            }

            for (int i = 0; i < 12; i++)
            {
                Neuron hiddenNeuron = hiddenNeurons[i];
                hiddenNeuron.Inputs = new double[5];

                for (int j = 0; j < 5; j++)
                {
                    double randValue = rand.NextDouble();
                    while (randValue == 0)
                    {
                        randValue = rand.NextDouble();
                    }
                    hiddenNeuron.Weights[j] = randValue;
                }
            }
           
            /*Neuron inputNeuron1 = new Neuron();
            inputNeuron1.Weights = new double[]{1};

            Neuron inputNeuron2 = new Neuron();
            inputNeuron2.Weights = new double[] { 1 };

            Neuron inputNeuron3 = new Neuron();
            inputNeuron3.Weights = new double[] { 1 };

            Neuron inputNeuron4 = new Neuron();
            inputNeuron4.Weights = new double[] { 1 };

            Neuron hiddenNeuron1 = new Neuron();
            hiddenNeuron1.Weights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                double randValue = rand.NextDouble();
                while (randValue == 0)
                {
                    randValue = rand.NextDouble();
                }
                hiddenNeuron1.Weights[i] = randValue;
            }

            Neuron hiddenNeuron2 = new Neuron();
            hiddenNeuron2.Weights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                double randValue = rand.NextDouble();
                while (randValue == 0)
                {
                    randValue = rand.NextDouble();
                }
                hiddenNeuron2.Weights[i] = randValue;
            }

            Neuron hiddenNeuron3 = new Neuron();
            hiddenNeuron3.Weights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                double randValue = rand.NextDouble();
                while (randValue == 0)
                {
                    randValue = rand.NextDouble();
                }
                hiddenNeuron3.Weights[i] = randValue;
            }

            Neuron hiddenNeuron4 = new Neuron();
            hiddenNeuron4.Weights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                double randValue = rand.NextDouble();
                while (randValue == 0)
                {
                    randValue = rand.NextDouble();
                }
                hiddenNeuron4.Weights[i] = randValue;
            }

            Neuron hiddenNeuron5 = new Neuron();
            hiddenNeuron5.Weights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                double randValue = rand.NextDouble();
                while (randValue == 0)
                {
                    randValue = rand.NextDouble();
                }
                hiddenNeuron5.Weights[i] = randValue;
            }

            Neuron hiddenNeuron6 = new Neuron();
            hiddenNeuron6.Weights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                double randValue = rand.NextDouble();
                while (randValue == 0)
                {
                    randValue = rand.NextDouble();
                }
                hiddenNeuron5.Weights[i] = randValue;
            }

            /*{
                rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), rand.NextDouble()
            };*/
        }
    }
}
