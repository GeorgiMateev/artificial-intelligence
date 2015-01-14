using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    public class CrossValidator
    {
        #region Construction
        public CrossValidator(
            IList<IDictionary<string, string>> samples, int partitions)
        {
            this.partitionsCount = partitions;

            var remaining = samples.Count % partitions;
            var partitionSize = samples.Count / partitions;

            if (remaining > 0)
            {
                partitionSize++;
            }

            this.partitions = CrossValidator.SplitList(samples.ToList(), partitionSize);
        }
        #endregion

        #region Public methods
        public IList<Tuple<double, DecisionTree>> GetMostAccurateTrees(
            IList<string> attributes,
            string classAttributeName,
            int minimumSamplesInTree)
        {
            var testResults = this.ValidateTrees(attributes, classAttributeName, minimumSamplesInTree);
            var orderedResults = testResults.OrderByDescending(x => x.Item1);
            return orderedResults.ToList();
        }        
        #endregion

        #region Private methods
        private static IList<IList<IDictionary<string, string>>> SplitList(List<IDictionary<string, string>> samples, int size)
        {
            IList<IList<IDictionary<string, string>>> list = new List<IList<IDictionary<string, string>>>();

            for (int i = 0; i < samples.Count; i += size)
            {
                list.Add(samples.GetRange(i, Math.Min(size, samples.Count - i)));
            }

            return list;
        }

        private IEnumerable<Tuple<IList<IDictionary<string, string>>, IList<IDictionary<string, string>>>> GetPartitionedDataSets()
        {
            for (int i = 0; i < this.partitionsCount; i++)
            {
                var validationSet = this.partitions[i];
                var trainingData = new List<IDictionary<string, string>>();

                for (int j = 0; j < this.partitionsCount; j++)
                {
                    if (i != j)
                    {
                        trainingData.AddRange(this.partitions[j]);
                    }
                }

                yield return new Tuple<IList<IDictionary<string, string>>, IList<IDictionary<string, string>>>(validationSet, trainingData);
            }
        }

        private IEnumerable<Tuple<IList<IDictionary<string, string>>, DecisionTree>> BuildDecisionTrees(
            IList<string> attributes,
            string classAttributeName,
            int minimumSamplesInTree)
        {
            var partitions = this.GetPartitionedDataSets();
            foreach (var partition in partitions)
            {
                var tree = new DecisionTree(partition.Item2, attributes, classAttributeName, minimumSamplesInTree);
                yield return new Tuple<IList<IDictionary<string, string>>, DecisionTree>(partition.Item1, tree);
            }
        }

        private IEnumerable<Tuple<double, DecisionTree>> ValidateTrees(
            IList<string> attributes,
            string classAttributeName,
            int minimumSamplesInTree)
        {
            var trees = this.BuildDecisionTrees(attributes, classAttributeName, minimumSamplesInTree);

            foreach (var tree in trees)
            {
                double successfullSuggestions = 0;

                var testData = tree.Item1;

                foreach (var sample in testData)
                {
                    var copy = new Dictionary<string, string>(sample);
                    copy.Remove(classAttributeName);

                    var suggestedClass = tree.Item2.Classify(copy);
                    if (suggestedClass == sample[classAttributeName])
                    {
                        successfullSuggestions++;
                    }
                }

                var successRate = successfullSuggestions / testData.Count * 100;

                yield return new Tuple<double, DecisionTree>(successRate, tree.Item2);
            }
        }
        #endregion

        #region Private fields and constants
        private IList<IList<IDictionary<string, string>>> partitions;
        private int partitionsCount;
        #endregion
    }
}
