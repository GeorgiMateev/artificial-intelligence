using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainDataFileName = "iris_train.txt";
            var data = new List<IEnumerable<double>>();

            using (var sr = new StreamReader(trainDataFileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var dataEntry = line.Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture));
                    data.Add(dataEntry);
                }
            }

            var hiddenLayerNeurons = 10;
            var iterations = 5;
            var net = new NeuralNetwork(4, 3, hiddenLayerNeurons);
            net.Train(data, iterations);
        }
    }

    public class NeuralNetwork
    {
        #region Construction
        public NeuralNetwork(int inputNeurons, int outputNeurons, int hiddenNeurons)
        {
            this.weights = new Matrix<double>[2];
            this.activations = new Vector<double>[3];

            this.InitWeights(inputNeurons, hiddenNeurons, 0);
            this.InitWeights(hiddenNeurons, outputNeurons, 1);
        }
        #endregion

        #region Public methods
        public void Train(IEnumerable<double> dataEntry)
        {
            var attributes = dataEntry.Take(dataEntry.Count() - 1);
            var attrVector = Vector<double>.Build.DenseOfEnumerable(attributes);
            this.ActivateLayers(attrVector);
        }

        public void Train(IList<IEnumerable<double>> data, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (var entry in data)
                {
                    this.Train(entry);
                }
            }
        }
        #endregion

        #region Private methods
        private void InitWeights(int firstLayerNeurons, int secondLayerNeurons, int index)
        {
            // Create matrix filled with random numbers from -0.005 to 0.005
            var weights = Matrix<double>.Build.Random(
                                                      secondLayerNeurons,
                                                      firstLayerNeurons,
                                                      new ContinuousUniform(-0.05, 0.05, new MersenneTwister(133)));

            this.weights[index] = weights;
        }

        private void ActivateLayers(Vector<double> input)
        {
            this.activations[0] = input;

            for (int i = 0; i < this.weights.Length; i++)
            {
                var activations = this.weights[i] * this.activations[i];
                this.activations[i + 1] = activations.Map(x => this.Logistic(x));
            }
        }

        private double Logistic(double sum)
        {
            return 1 / (1 + Math.Pow(Math.E, (-sum))); 
        }
        #endregion

        #region Private fields and constants
        private Matrix<double>[] weights;
        private Vector<double>[] activations;
        #endregion
    }
}
