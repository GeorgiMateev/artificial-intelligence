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
            var q = new SortedList<int, Node>();
            var visited = new HashSet<Node>();

            var startHeuristics = Heuristics(startState);

            var startNode = new Node(1, startHeuristics, startState);
            startNode.ZeroIndex = FindZeroCoordinates(startState);

            q.Add(startHeuristics + 1, startNode);

            visited.Add(startState);

            KeyValuePair<int, Node> current;
            while (q.Count > 0)
            {
                current = Pop(q);

                if (current.Value.Heuristics == 0)
                {
                    break;
                }

                var neighbours = Neighbours(current.Value);

                foreach (var neighbour in neighbours)
                {
                    if (visited.Contains(neighbour.State))
                    {
                        continue;
                    }

                    neighbour.Path = current.Value.Path + 1;
                    neighbour.Heuristics = Heuristics(neighbour.State);

                    q.Add(neighbour.Path + neighbour.Heuristics, neighbour);
                }
            }
        }

        private static IList<Node> Neighbours(Node node)
        {
        }

        private static KeyValuePair<int,int[,]> Pop(SortedList<int, int[,]> q)
        {
            var element = q.Last();
            q.RemoveAt(q.Count - 1);
            return element;
        }

        private static int Heuristics(int[,] state)
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

        private static Point FindZeroCoordinates(int[,] state)
        {
            var l = state.Length;

            for (int y = 0; y < l; y++)
            {
                for (int x = 0; x < l; x++)
                {
                    if(state[y, x] == 0)
                    {
                        return new Point(x, y);
                    }
                }
            }
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
            public int Heuristics { get; set; }
            public int[,] State { get; set; }

            public int ZeroIndex { get; set; }

            public Node(int path, int heuristics, int[,] state)
            {
                this.Path = path;
                this.Heuristics = heuristics;
                this.State = state;
            }

            public Node(int[,] state)
            {
                this.State = state;
            }
        }
    }

    
}
