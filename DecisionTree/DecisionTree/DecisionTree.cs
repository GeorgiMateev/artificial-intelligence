using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    public class DecisionTree : ITree
    {
        #region Construction
        public DecisionTree(
            IList<IDictionary<string, string>> samples,
            IList<string> attributes,
            string classAttributeName,
            int minimumSamplesInTree)
        {
            this.minimumSamplesInTree = minimumSamplesInTree;
            this.attributes = attributes;
            this.classAttributeName = classAttributeName;

            var attr = this.SelectRootAttribute(attributes, samples);
            this.Root = new TreeNode(attr, null);
            this.ConstructTree(this.Root, this.attributes, samples);
        }
        #endregion

        #region ITree implementation
        public ITreeNode Root { get; set; }
        #endregion

        #region Private methods
        private double Entropy(IList<IDictionary<string, string>> samples)
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

        private double InformationGain(string attribute, IList <IDictionary<string, string>> samples)
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

        private string SelectRootAttribute(IList<string> attributes, IList<IDictionary<string, string>> samples)
        {
            var attr = attributes.Select(a =>
                new Tuple<string, double>(a, this.InformationGain(a, samples)))
                .OrderByDescending(t => t.Item2)
                .First()
                .Item1;

            return attr;
        }

        private void ConstructTree(ITreeNode root, IList<string> attributes, IList<IDictionary<string, string>> samples)
        {
            if (root.IsLeaf)
	        {
		        return;
	        }

            var groups = samples.GroupBy(s => s[root.Attribute]);
            foreach (var group in groups)
            {
                var groupItems = group.ToList();

                ITreeNode node;
                if (groupItems.Count > this.minimumSamplesInTree && attributes.Count > 0)
                {
                    var attr = this.SelectRootAttribute(attributes, groupItems);
                    node = new TreeNode(attr, group.Key);
                }
                else
	            {
                    var samplesClass = this.DetermineClass(groupItems);
                    node = new TreeNode(samplesClass, group.Key, true);                    
	            }
                root.AddChild(node);

                attributes.Remove(root.Attribute);

                this.ConstructTree(node, attributes, groupItems);
            }
        }

        private string DetermineClass(List<IDictionary<string, string>> samples)
        {
            return samples
                .GroupBy(s => s[this.classAttributeName])
                .OrderByDescending(g => g.Key)
                .First()
                .Key;
        }
        #endregion

        #region Private fields and constants
        private string classAttributeName;
        private IList<string> attributes;
        private int minimumSamplesInTree;
        #endregion        
    }
}
