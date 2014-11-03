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
                var selected = CrossOver(population);
                Mutate(selected);                

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
            throw new NotImplementedException();
        }

        private static void Mutate(IList<Point[]> selected)
        {
            throw new NotImplementedException();
        }

        private static IList<Point[]> GeneratePopulation(Point[] points, int populationSize)
        {
            throw new NotImplementedException();
        }

        private static Point[] GeneratePoints(int n)
        {
            throw new NotImplementedException();
        }

        public struct Point
        {
            public Point (int x, int y) 
                : this()
	        {
                this.X = x;
                this.Y = y;
        	}

            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
