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
            var learningConst = 0.1;
            var net = new NeuralNetwork(4, 3, hiddenLayerNeurons);
            net.Train(data, iterations, learningConst);
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
        public void Train(IEnumerable<double> dataEntry, double learningConst)
        {
            var attributes = dataEntry.Take(dataEntry.Count() - 1);
            var attrVector = Vector<double>.Build.DenseOfEnumerable(attributes);
            this.ActivateLayers(attrVector);

            var expectedClass = (int)dataEntry.Last();
            var expectedOutput = this.ExpectedOutput(expectedClass, this.activations[2].Count);

            var outputErrors = this.OutputErrors(expectedOutput);
            var hiddenLayerErrors = this.HiddenLayerErrors(this.activations[1], outputErrors);

            this.UpdateWeights(this.weights[1], activations[1], outputErrors, learningConst);
            this.UpdateWeights(this.weights[0], activations[0], hiddenLayerErrors, learningConst);
        }

        public void Train(IList<IEnumerable<double>> data, int iterations, double learningConst)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (var entry in data)
                {
                    this.Train(entry, learningConst);
                }
            }
        }

        public Vector<double> GetOutput()
        {
            return this.activations.Last();
        }
        #endregion

        #region Private methods
        private void InitWeights(int firstLayerNeurons, int secondLayerNeurons, int index)
        {
            // Create matrix filled with random numbers from -0.005 to 0.005
            var weights = Matrix<double>.Build.Random(secondLayerNeurons,
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

        private Vector<double> ExpectedOutput(int expectedClass, int outputNeurons)
        {
            var output = Vector<double>.Build.Sparse(outputNeurons);
            output[expectedClass - 1] = 1.0;
            return output;
        }

        private Vector<double> OutputErrors(Vector<double> expected)
        {
            var output = this.GetOutput();
            return output.MapIndexed((i, x) => x * (1 - x) * (expected[i] - x));
        }

        private Vector<double> HiddenLayerErrors(Vector<double> activations, Vector<double> outputErrors)
        {
            var errors = this.weights[1].Transpose() * outputErrors.ToColumnMatrix();
            var vector = errors.EnumerateColumns().First();
            vector.MapIndexedInplace((i, x) => x * (1 - x) * activations[i]);
            return vector;
        }

        private void UpdateWeights(Matrix<double> weights, Vector<double> activations, Vector<double> errors, double learningConst)
        {
            var rows = weights.EnumerateRowsIndexed();

            foreach (var row in rows)
            {
                var error = errors[row.Item1];
                row.Item2.MapIndexedInplace((i, x) => x + learningConst * error * activations[i]);
            }
        }
        #endregion

        #region Private fields and constants
        private Matrix<double>[] weights;
        private Vector<double>[] activations;
        #endregion
    }
}
