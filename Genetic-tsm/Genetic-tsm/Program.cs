using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Genetic_tsm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Number of cities:");
            var n = int.Parse(Console.ReadLine());
            var points = GeneratePoints(n);

            Console.WriteLine("Population size:");
            var population = int.Parse(Console.ReadLine());

            Console.WriteLine("Epochs:");
            var epochs = int.Parse(Console.ReadLine());

            Console.WriteLine("Mutation rate:");
            var mutationRate = int.Parse(Console.ReadLine());

            Genetic(points, population, epochs, mutationRate);
        }        

        private static void Genetic(Point[] points, int populationSize, int epochs, int mutationRate)
        {
            var population = GeneratePopulation(points, populationSize);

            for (int i = 0; i < epochs; i++)
            {                
                var crossed = CrossOver(population);

                var mutated = Mutate(crossed, mutationRate);

                population.AddRange(mutated);

                population = Evolution(population, populationSize);

                // Print the length of the shortest path after the 10th, 1/4, 2/4, 3/4 and the last epoch.
                if (i == 9 ||
                    i == Math.Floor(epochs * 1 / 4d) ||
                    i == Math.Floor(epochs * 1 / 2d) ||
                    i == Math.Floor(epochs * 3 / 4d) ||
                    i == epochs -1)
                {
                    var shortestPath = population[0];
                    var length = Fitness(shortestPath);
                    Console.WriteLine("On {0} epoch the shortest path length is: {1}", i + 1, length);
                }
            }
        }

        private static IList<Point[]> Mutate(IList<Point[]> crossed, int mutationRate)
        {
            var r = new Random();
            for (int i = 0; i < crossed.Count; i++)
            {
                if (r.Next(1, 101) <= mutationRate)
                {
                    // Reverse two random positions in the array
                    var path = crossed[i];
                    var firstIndex = r.Next(path.Length);
                    var secondIndex = r.Next(path.Length);

                    var firstIndexCopy = path[firstIndex];
                    path[firstIndex] = path[secondIndex];
                    path[secondIndex] = firstIndexCopy;
                }                
            }

            return crossed;
        }

        /// <summary>
        /// Return the paths with shortest length.
        /// </summary>
        /// <param name="population"></param>
        /// <param name="populationSize"></param>
        private static List<Point[]> Evolution(IList<Point[]> population, int populationSize)
        {
            var sortedPopulation = new SortedDictionary<double, Point[]>(new DuplicateKeyComparer<double>());
            for (int i = 0; i < population.Count; i++)
            {
                sortedPopulation.Add(
                    Fitness(population[i]),
                    population[i]);
            }

            var survived = sortedPopulation.Values
                .Take(populationSize)
                .ToList();
            return survived;
        }

        /// <summary>
        /// Calculate the length of the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static double Fitness(Point[] path)
        {
            var length = 0d;
            for (int i = 0; i < path.Length - 1; i++)
            {
                length += Distance(path[i], path[i + 1]);
            }

            length += Distance(path[path.Length - 1], path[0]);

            return length;
        }

        private static double Distance(Point point1, Point point2)
        {
            var xDelta = point1.X - point2.X;
            var yDelta = point1.Y - point2.Y;

            return Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));
        }

        private static IList<Point[]> CrossOver(IList<Point[]> population)
        {
            var randomListTool = new RandomListTool<Point[]>();
            var shuffled = randomListTool.ShuffleList(population);

            var crossed = new List<Point[]>();
            var r = new Random();
            for (int i = 0; i < shuffled.Count / 2; i++)
            {
                var mPath = shuffled[i];
                var fPath = shuffled[i + 1];

                var pivot = r.Next(mPath.Length);

                var sorted = new SortedDictionary<int, Point>();
                for (int e = pivot; e < mPath.Length; e++)
                {
                    var element = mPath[e];
                    var elementIndex = FindIndex(fPath, element);
                    sorted.Add(elementIndex, element);
                }

                var crossedPath = new List<Point>();
                for (int firstPartIndex = 0; firstPartIndex < pivot; firstPartIndex++)
                {
                    crossedPath.Add(mPath[firstPartIndex]);
                }

                crossedPath.AddRange(sorted.Values.ToList());

                crossed.Add(crossedPath.ToArray());
            }

            return crossed;
        }

        private static int FindIndex(Point[] population, Point element)
        {
            for (int i = 0; i < population.Length; i++)
            {
                if (population[i].Id == element.Id)
                {
                    return i;
                }
            }

            throw new InvalidOperationException();
        }

        private static List<Point[]> GeneratePopulation(Point[] points, int populationSize)
        {
            var population = new List<Point[]>();
            var randomListTool = new RandomListTool<Point>();

            for (int i = 0; i < populationSize; i++)
            {
                population.Add(randomListTool.ShuffleList(points).ToArray());
            }

            return population;
        }

        private static Point[] GeneratePoints(int n)
        {
            var r = new Random();
            var maxDistance = 1000;
            var points = new Point[n];

            for (int i = 0; i < n; i++)
            {
                var x = r.Next(maxDistance);
                var y = r.Next(maxDistance);
                points[i] = new Point(x, y, r.Next(10000));
            }

            return points;
        }

        [DebuggerDisplay("Id: {Id}, X: {X}, Y: {Y}")]
        public struct Point
        {
            public Point (int x, int y, int id) 
                : this()
	        {
                this.X = x;
                this.Y = y;
                this.Id = id;
        	}

            public int X { get; set; }
            public int Y { get; set; }
            public int Id { get; set; }
        }

        public class DuplicateKeyComparer<TKey>
                : IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1;   // Handle equality as beeing greater
                else
                    return result;
            }

            #endregion
        }

        public class RandomListTool<T>
        {
            static Random _random = new Random();

            public  IList<T> ShuffleList(IList<T> list)
            {
                var randomMap = new List<KeyValuePair<int, T>>();

                foreach (var s in list)
                {
                    randomMap.Add(new KeyValuePair<int, T>(_random.Next(), s));
                }

                // Sort the list by the random number
                var sorted = from item in randomMap
                             orderby item.Key
                             select item.Value;

                return sorted.ToList<T>();
            }
        }
    }
}
