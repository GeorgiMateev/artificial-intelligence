using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    public interface ITreeNode
    {
        IList<ITreeNode> Children { get; set; }

        string Attribute { get; set; }

        string ParentValue { get; set; }

        bool IsLeaf { get; set; }

        double Probability { get; set; }

        void AddChild(ITreeNode child);
    }
}
