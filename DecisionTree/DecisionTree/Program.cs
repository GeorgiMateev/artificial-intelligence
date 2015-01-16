using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainDataFileName = "breast-cancer.arff";
            
            string className;
            var attributes = Program.ReadAttributes(trainDataFileName, out className);

            var data = Program.ReadData(trainDataFileName, attributes, className);

            var minimumSamplesInTree = 3;

            var validator = new CrossValidator(data, 10);
            var trees = validator.GetMostAccurateTrees(attributes, className, minimumSamplesInTree);

            Console.WriteLine("Percentage of success after cross validation:");
            for (var i = 0; i < trees.Count; i++)
            {
                Console.WriteLine("Tree {0}: {1}%", i, trees[i].Item1);
            }

            //var tree = new DecisionTree(data, attributes, className, minimumSamplesInTree);
        }

        private static IList<IDictionary<string, string>> ReadData(string fileName, IList<string> attributes, string className)
        {
            IList<IDictionary<string, string>> data = new List<IDictionary<string, string>>();

            using (var sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line[0] != '@' && line[0] != '%')
                    {
                        var entry = new Dictionary<string, string>();
                        var lineParts = line.Split(',');
                        for (int i = 0; i < attributes.Count; i++)
                        {
                            entry.Add(attributes[i], lineParts[i]);
                        }
                        entry.Add(className, lineParts[lineParts.Length - 1]);
                        data.Add(entry);
                    }
                }
            }
            return data;
        }

        private static IList<string> ReadAttributes(string fileName, out string classAttributeName)
        {
            var attrs = new List<string>();

            using(var sr = new StreamReader(fileName))
	        {
		        string line;
                while ((line = sr.ReadLine()) != null)
	            {                        
                    if (line.StartsWith("@attribute"))
                    {
                        var lineParts = line.Split(' ');
                        attrs.Add(lineParts[1]);
                    }
	            }
	        }

            // the last attribute is the class name
            classAttributeName = attrs.Last();
            attrs.RemoveAt(attrs.Count - 1);

            return attrs;
        }
    }

    
}
