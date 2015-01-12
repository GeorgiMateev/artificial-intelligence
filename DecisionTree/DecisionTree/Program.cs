using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class DecisionTree
    {
        #region Construction

        #endregion

        #region Private methods
        private double Entropy(ISet<IDictionary<string, string>> samples)
        {
            var samplesCount = samples.Count;

            double entropy = 0;
            var groups = samples.GroupBy(s => s[this.classAttributeName]);
            foreach (var group in groups)
            {
                var probability = group.Count() / samplesCount;

                entropy += probability * Math.Log10(probability);
            }

            return -entropy;
        }

        private double InformationGain(string attribute, ISet<IDictionary<string, string>> samples)
        {
            var samplesEntropy = this.Entropy(samples);

            var groups = samples.GroupBy(s => s[attribute]);
            double childrenEntropy = 0;
            foreach (var group in groups)
	        {
                childrenEntropy += this.Entropy(samples);		 
	        }

            return samplesEntropy - childrenEntropy;
        }

        private string SelectRootAttribute(IList<string> attributes, ISet<IDictionary<string, string>> samples)
        {
            var attr = attributes.Select(a => 
                new Tuple<string, double>(a, this.InformationGain(a, samples)))
                .OrderByDescending(t => t.Item2)
                .First()
                .Item1;

            return attr;
        }
        #endregion

        #region Private fields and constants
        private string classAttributeName;
        private IList<string> classes;
        #endregion
    }
}
