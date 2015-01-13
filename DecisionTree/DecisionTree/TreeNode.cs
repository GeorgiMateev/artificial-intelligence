using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    public class TreeNode : ITreeNode
    {
        #region Construction
        public TreeNode(string attribute, string parentValue)
        {
            this.Attribute = attribute;
            this.ParentValue = parentValue;
            this.Children = new List<ITreeNode>();
        }

        public TreeNode(string attribute, string parentValue, bool isLeaf)
        {
            this.Attribute = attribute;
            this.ParentValue = parentValue;
            this.IsLeaf = IsLeaf;
            this.Children = new List<ITreeNode>();
        }
        #endregion

        #region ITreeNode implementation
        public IList<ITreeNode> Children { get; set; }

        public string Attribute { get; set; }

        public string ParentValue { get; set; }

        public bool IsLeaf { get; set; }

        public void AddChild(ITreeNode child)
        {
            this.Children.Add(child);
        }
        #endregion
    }
}
