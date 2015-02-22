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
                {1,1,1,1},
                {1,0,0,1},
                {0,1,1,0},
                {0,0,1,1},
                {0,1,1,0},
                {0,1,0,1},
                {0,1,1,0},
                {1,1,1,1},
                {1,0,0,1},
                {1,0,0,1},
                {1,0,1,0},
                {1,1,1,1},
                {1,1,0,0},
                {1,1,1,1},
                {1,1,1,1},
                {1,1,1,1}
            };

            Neuron inputNeuron1 = new Neuron();
            inputNeuron1.Weights = new double[]{1};

            Neuron inputNeuron2 = new Neuron();
            inputNeuron2.Weights = new double[] { 1 };

            Neuron inputNeuron3 = new Neuron();
            inputNeuron3.Weights = new double[] { 1 };

            Neuron inputNeuron4 = new Neuron();
            inputNeuron4.Weights = new double[] { 1 };

            Neuron hiddenNeuron1 = new Neuron();
            hiddenNeuron1.Inputs = new double[]
            {
                inputNeuron1.Output(), inputNeuron2.Output(), inputNeuron3.Output(), inputNeuron4.Output()
            };
        }
    }
}
