using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Genetic_tsm
{
    class Program
    {
        static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            var points = GeneratePoints(n);

            Genetic(points, 100, 100);
        }        

        private static void Genetic(Point[] points, int populationSize, int epochs)
        {
            var population = GeneratePopulation(points, populationSize);

            for (int i = 0; i < epochs; i++)
            {                
                var crossed = CrossOver(population);
                var mutated = Mutate(crossed);

                population.AddRange(mutated);

                Evolution(population, populationSize);
            }
        }

        /// <summary>
        /// Return the paths with shortest length.
        /// </summary>
        /// <param name="population"></param>
        /// <param name="populationSize"></param>
        private static void Evolution(IList<Point[]> population, int populationSize)
        {
            var sortedPopulation = new SortedDictionary<int, Point[]>();
            for (int i = 0; i < population.Count; i++)
            {
                sortedPopulation.Add(
                    Fitness(population[i]),
                    population);
            }

            var survived = population.Reverse().Take(populationSize);
            return survived;
        }

        /// <summary>
        /// Calculate the length of the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static int Fitness(Point[] path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Group by two.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        private static IList<Point[]> CrossOver(IList<Point[]> population)
        {
            var cross = new List<Point>();
            var r = new Random();
            for (int i = 0; i < population.Count; i++)
            {
                var index = r.Next(population.Count);
                cross.Add(population[index]);
            }
        }

        private static IList<Point[]> Mutate(IList<Point[]> crossed)
        {
            var mutated = new List<Point[]>();
            var r = new Random();
            for (int i = 0; i < crossed.Count / 2; i++)
            {
                var mPath = crossed[i];
                var fPath = crossed[i + 1];
                
                var pivot = r.Next(mPath.Count);

                var sorted = new SortedDictionary<int, Point>;
                for (int e = pivot; e < mPath.Count; e++)
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

                mutated.Add(mutatedPath);
            }

            return mutated;
        }

        private static int FindIndex(Point[] population, Point element)
        {
            for (int i = 0; i < population.Length; i++)
            {
                if (population[i].Id = element.Id)
                {
                    return i;
                }
            }

            throw new InvalidOperationException();
        }

        private static List<Point[]> GeneratePopulation(Point[] points, int populationSize)
        {
            throw new NotImplementedException();
        }

        private static Point[] GeneratePoints(int n)
        {
            throw new NotImplementedException();
        }

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
    }
}
