using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numbers
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        static void AStar(int[,] startState)
        {
            var q = new SortedList<int, int[,]>();
            var visited = new HashSet<int[,]>();

            var startIndex = Euristics(startState);
            q.Add(startIndex, startState);

            visited.Add(startState);

            KeyValuePair<int, int[,]> current;
            while (q.Count > 0)
            {
                current = Pop(q);

                if (current.Key == 0)
                {
                    break;
                }

                var neighbours = Neighbours(current);

                foreach (var n in neighbours)
                {
                    if (visited.Contains(n.Value))
                    {
                        continue;
                    }

                    //TODO: every node should remember the lenght of the path to itself.
                    q.Add(n.Key /* + path*/, n.Value);
                }
            }
        }

        private static IList<KeyValuePair<int, int[,]>> Neighbours(KeyValuePair<int, int[,]> current)
        {
            throw new NotImplementedException();
        }

        private static KeyValuePair<int,int[,]> Pop(SortedList<int, int[,]> q)
        {
            var element = q.Last();
            q.RemoveAt(q.Count - 1);
            return element;
        }

        private static int Euristics(int[,] state)
        {
            //It's a square.
            var length = state.GetLength(0);

            int pathToGo = 0;

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    var currentNumber = state[y, x];
                    Point coordinates = GetFinalCoordinatesOfNumber(currentNumber, length);

                    //Add the length of the Manhatan path.
                    pathToGo += Math.Abs(coordinates.X - x) + Math.Abs(coordinates.Y - y);
                }
            }

            return pathToGo;
        }

        private static Point GetFinalCoordinatesOfNumber(int currentNumber, int dimensionLength)
        {
            Point c;
            if (coordinates.TryGetValue(currentNumber, out c))
            {
                return c;
            }

            if (currentNumber == 0)
            {
                c = new Point(dimensionLength - 1, dimensionLength - 1);
                coordinates[currentNumber] = c;
                return c;
            }

            var y = currentNumber / dimensionLength - 1;
            var x = currentNumber % dimensionLength - 1;

            c = new Point(x, y);
            coordinates[currentNumber] = c;
            return c;
        }
        
        private static IDictionary<int, Point> coordinates = new Dictionary<int, Point>();

        struct Point 
        {
            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public int X { get; set; }
            public int Y { get; set; }
        }

        struct Node
        {
            public int Path { get; set; }
            public int Euristic { get; set; }
            public int[,] State { get; set; }
        }
    }

    
}
