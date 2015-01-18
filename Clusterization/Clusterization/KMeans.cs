using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clusterization
{
    public class KMeans
    {
        #region Public methods
        public IList<Tuple<double, double, int>> Clusterize(IList<Tuple<double, double, int>>data, int clusters, int runs, out IList<Tuple<double,double>> initialMeans, out IList<Tuple<double,double>> centroids)
        {
            double minInnerDistance = double.MaxValue;
            IList<Tuple<double, double, int>> bestRunData = null;
            initialMeans = null;
            centroids = null;

            for (int i = 0; i < runs; i++)
            {
                IList<Tuple<double, double>> tempInitialMeans;
                IList<Tuple<double, double>> tempCentroids;

                this.ClusterizeInternal(data, clusters, out tempInitialMeans, out tempCentroids);

                var innerDistance = this.CalculateInnerDistance(data, tempCentroids);
                if (innerDistance < minInnerDistance)
                {
                    minInnerDistance = innerDistance;
                    bestRunData = new List<Tuple<double, double, int>>(data);
                    initialMeans = new List<Tuple<double, double>>(tempInitialMeans);
                    centroids = new List<Tuple<double, double>>(tempCentroids); 
                }
            }
            return bestRunData;
        }                            
        #endregion

        #region Private methods
        private void ClusterizeInternal(IList<Tuple<double, double, int>> data, int clusters, out IList<Tuple<double, double>> initialMeans, out IList<Tuple<double, double>> centroids)
        {
            centroids = this.ChooseInitialMeans(data, clusters);
            initialMeans = new List<Tuple<double, double>>(centroids);

            var assigned = true;
            while (assigned)
            {
                assigned = this.AssignClusters(data, centroids);

                this.UpdateCentroids(data, centroids);
            }
        }

        private IList<Tuple<double, double>> ChooseInitialMeans(IList<Tuple<double, double, int>> data, int clusters)
        {
            var means = new List<Tuple<double, double>>();
            for (int i = 0; i < clusters; i++)
            {
                var sample = data[KMeans.random.Next(data.Count)];
                var mean = new Tuple<double, double>(sample.Item1, sample.Item2);
                means.Add(mean);
            }
            return means;
        }

        private bool AssignClusters(IList<Tuple<double, double, int>> data, IList<Tuple<double, double>> centroids)
        {
            var assigned = false;
            for (var i = 0; i < data.Count; i++)
            {
                var sample = data[i];
                var cluster = this.AssignCluster(sample, centroids);
                if (cluster != 0)
	            {
		            var newSample = new Tuple<double, double, int>(sample.Item1, sample.Item2, cluster);
                    data[i] = newSample;
                    assigned = true;
	            }               
            }

            return assigned;
        }  

        private int AssignCluster(Tuple<double, double, int> sample, IList<Tuple<double, double>> centroids)
        {
            double minDistance = int.MaxValue;
            var cluster = 0;
            for (int i = 0; i < centroids.Count; i++)
            {
                var c = centroids[i];
                var distance = this.Distance(sample, c);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    cluster = i + 1;
                }
            }

            if(sample.Item3 != cluster)
            {
                return cluster;
            }

            return 0;
        }

        private void UpdateCentroids(IList<Tuple<double, double, int>> data, IList<Tuple<double, double>> centroids)
        {
            var clusters = data.GroupBy(s => s.Item3).ToList();

            for (var i = 0; i < clusters.Count(); i++)
            {
                var centroid = this.CalculateCentroid(clusters[i].ToList());
                centroids[i] = centroid;
            }
        }

        private double Distance(Tuple<double, double, int> p1, Tuple<double, double> p2)
        {
            return Math.Sqrt(Math.Pow(p1.Item1 - p2.Item1, 2) + Math.Pow(p1.Item2 - p2.Item2, 2));
        }

        private Tuple<double, double> CalculateCentroid(IList<Tuple<double, double, int>> cluster)
        {
            var centroid = new Tuple<double, double>(cluster[0].Item1, cluster[0].Item2);

            for (int i = 1; i < cluster.Count; i++)
			{
                var point = cluster[i];
                var x = centroid.Item1 + ((point.Item1 - centroid.Item1) / ((double)i + 1));
                var y = centroid.Item2 + ((point.Item2 - centroid.Item2) / ((double)i + 1));
			    centroid = new Tuple<double, double>(x, y);
			}

            return centroid;
        }

        private double CalculateInnerDistance(IList<Tuple<double, double, int>> data, IList<Tuple<double, double>> centroids)
        {
            double distanceSum = 0;
            foreach (var sample in data)
            {
                var cluster = sample.Item3 -1;
                var distance = this.Distance(sample, centroids[cluster]);
                distanceSum += distance;
            }
            return distanceSum;
        } 
        #endregion

        #region Private fields and constants
        private static Random random = new Random();
        #endregion
    }
}
