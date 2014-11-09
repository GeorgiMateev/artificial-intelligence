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

            Genetic(points, population, epochs);
        }        

        private static void Genetic(Point[] points, int populationSize, int epochs)
        {
            var population = GeneratePopulation(points, populationSize);

            for (int i = 0; i < epochs; i++)
            {                
                var crossed = CrossOver(population);
                var mutated = Mutate(crossed);

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

        /// <summary>
        /// Group by two.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        private static IList<Point[]> CrossOver(IList<Point[]> population)
        {
            var cross = new List<Point[]>();
            var r = new Random();
            for (int i = 0; i < population.Count; i++)
            {
                var index = r.Next(population.Count);
                cross.Add(population[index]);
            }

            return cross;
        }

        private static IList<Point[]> Mutate(IList<Point[]> crossed)
        {
            var mutated = new List<Point[]>();
            var r = new Random();
            for (int i = 0; i < crossed.Count / 2; i++)
            {
                var mPath = crossed[i];
                var fPath = crossed[i + 1];
                
                var pivot = r.Next(mPath.Length);

                var sorted = new SortedDictionary<int, Point>();
                for (int e = pivot; e < mPath.Length; e++)
                {
                    var element = mPath[e];
                    var elementIndex = FindIndex(fPath, element);
                    sorted.Add(elementIndex, element);
                }

                var mutatedPath = new List<Point>();
                for (int firstPartIndex = 0; firstPartIndex < pivot; firstPartIndex++)
			    {
			        mutatedPath.Add(mPath[firstPartIndex]);
			    }

                mutatedPath.AddRange(sorted.Values.ToList());

                mutated.Add(mutatedPath.ToArray());
            }

            return mutated;
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

            for (int i = 0; i < populationSize; i++)
            {
                population.Add(RandomArrayTool.RandomizeArray(points));
            }

            return population;
        }

        private static Point[] GeneratePoints(int n)
        {
            var r = new Random();
            var maxDistance = 100;
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

        static class RandomArrayTool
        {
            static Random _random = new Random();

            public static Point[] RandomizeArray(Point[] arr)
            {
                var list = new List<KeyValuePair<int, Point>>();
                // Add all strings from array
                // Add new random int each time
                foreach (var s in arr)
                {
                    list.Add(new KeyValuePair<int, Point>(_random.Next(), s));
                }
                // Sort the list by the random number
                var sorted = from item in list
                             orderby item.Key
                             select item;
                // Allocate new string array
                var result = new Point[arr.Length];
                // Copy values to array
                int index = 0;
                foreach (KeyValuePair<int, Point> pair in sorted)
                {
                    result[index] = pair.Value;
                    index++;
                }
                // Return copied array
                return result;
            }
        }
    }
}
