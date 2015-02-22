using System;
using System.Linq;

namespace NeuralNetworkEdgeDetection
{
    class Neuron
    {
        public double[] Inputs { get; set; }
        public double[] Weights { get; set; }

        public Neuron[] Predecessors { get; set; }

        //public Neuron(int number, bool input) { }

        //todo: bias?
        public double Output()
        {
            double x = 0;

            for (int i = 0; i < Inputs.Count(); i++)
            {
                x += Inputs[i]*Weights[i];
            }

            double output = 1/(1 + Math.Exp(-x));
            return output;
        }
    }
}
