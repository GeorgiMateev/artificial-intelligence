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
            this.ConstructTree(this.Root, new List<string>(this.attributes), samples);
        }
        #endregion

        #region ITree implementation
        public ITreeNode Root { get; set; }
        #endregion

        #region Public methods
        public string Classify(IDictionary<string, string> sample)
        {
            return this.ClassifyInternal(this.Root, sample);
        }
        #endregion

        #region Private methods
        private string ClassifyInternal(ITreeNode node, IDictionary<string, string> sample)
        {
            if (node.IsLeaf)
            {
                return node.Attribute;
            }

            var attrValue = sample[node.Attribute];

            var nextNode = node.Children.FirstOrDefault(c => c.ParentValue == attrValue);

            if (attrValue == "?" || nextNode == null)
	        {
		        nextNode = node.Children.OrderBy(c => c.Probability).First();
	        }

            return this.ClassifyInternal(nextNode, sample);
        }

        private double Entropy(IList<IDictionary<string, string>> samples)
        {
            double samplesCount = samples.Count;

            double entropy = 0;
            var groups = samples.GroupBy(s => s[this.classAttributeName]);
            foreach (var group in groups)
            {
                var probability = group.Count() / samplesCount;

                entropy += probability * Math.Log(probability);
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
                childrenEntropy += (group.Count() / (double)samples.Count) * this.Entropy(group.ToList());
            }

            return samplesEntropy - childrenEntropy;
        }

        private string SelectRootAttribute(IList<string> attributes, IList<IDictionary<string, string>> samples)
        {
            var orderedAttrs = attributes.Select(a =>
                new Tuple<string, double>(a, this.InformationGain(a, samples)))
                .OrderByDescending(t => t.Item2).ToList();

            return orderedAttrs
                .First()
                .Item1;;
        }

        private void ConstructTree(ITreeNode root, IList<string> attributes, IList<IDictionary<string, string>> samples)
        {
            if (root.IsLeaf)
	        {
		        return;
	        }

            attributes.Remove(root.Attribute);

            var groups = samples.GroupBy(s => s[root.Attribute]);
            foreach (var group in groups)
            {
                if (group.Key == "?")
                {
                    continue;
                }

                var groupItems = group.ToList();

                ITreeNode node;
                if (groupItems.Count > this.minimumSamplesInTree && attributes.Count > 0 && !this.HasOneClass(groupItems))
                {
                    var attr = this.SelectRootAttribute(attributes, groupItems);
                    var probability = groupItems.Count / (double)samples.Count;
                    node = new TreeNode(attr, group.Key, probability);
                }
                else
	            {
                    var samplesClass = this.DetermineClass(groupItems);
                    var probability = groupItems.Count / (double)samples.Count;
                    node = new TreeNode(samplesClass, group.Key, true, probability);                    
	            }
                root.AddChild(node);

                var attrsCopy = new List<string>(attributes);
                this.ConstructTree(node, attrsCopy, groupItems);
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

        private bool HasOneClass(List<IDictionary<string, string>> samples)
        {
            return samples.GroupBy(s => s[this.classAttributeName]).Count() == 1;
        }
        #endregion

        #region Private fields and constants
        private string classAttributeName;
        private IList<string> attributes;
        private int minimumSamplesInTree;
        #endregion        
    }
}
