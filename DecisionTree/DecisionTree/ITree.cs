using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    public interface ITree
    {
        ITreeNode Root { get; set; }
    }
}
