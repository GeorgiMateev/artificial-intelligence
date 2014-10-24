using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numbers
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new int[,]
            {
                {6, 5, 3},
                {2, 4, 8},
                {7, 0, 1}
            };

            var finalNode = AStar(input);
            Console.WriteLine("The path is: ", finalNode.Path);

            var l = finalNode.State.GetLength(0);
            var state = finalNode.State;
            for (int y = 0; y < l; y++)
            {
                for (int x = 0; x < l; x++)
                {
                    Console.Write(state[y, x] + " ");
                }
                Console.WriteLine();
            }
        }

        static Node AStar(int[,] startState)
        {
            var q = new SortedList<int, Node>(new DuplicateKeyComparer<int>());
            var visited = new HashSet<int[,]>(new StateEqualityComparer<int[,]>());

            var startHeuristics = Heuristics(startState);

            var startNode = new Node(1, startHeuristics, startState);
            startNode.ZeroIndex = FindZeroCoordinates(startState);

            q.Add(startHeuristics + 1, startNode);

            visited.Add(startState);

            KeyValuePair<int, Node> current;
            while (q.Count > 0)
            {
                current = Pop(q);
                visited.Add(current.Value.State);

                if (current.Value.Heuristics == 0)
                {
                    return current.Value;
                }

                var neighbours = Neighbours(current.Value);

                for (var i = 0; i < neighbours.Count; i++)
                {
                    var neighbour = neighbours[i];
                    if (visited.Contains(neighbour.State))
                    {
                        continue;
                    }

                    neighbour.Path = current.Value.Path + 1;
                    neighbour.Heuristics = Heuristics(neighbour.State);

                    q.Add(neighbour.Path + neighbour.Heuristics, neighbour);                   
                }
            }

            throw new ApplicationException("Didn't found a solution.");
        }

        private static IList<Node> Neighbours(Node node)
        {
            var neighbours = new List<Node>();
            var l = node.State.GetLength(0);
            var zero = node.ZeroIndex;

            var directions = new Point[]
            {
                new Point(zero.X + 1, zero.Y),
                new Point(zero.X, zero.Y + 1),
                new Point(zero.X - 1, zero.Y),
                new Point(zero.X, zero.Y -1)
            };

            foreach (var direction in directions)
	        {
		                   
                if (direction.X < l && direction.X >= 0
                    && direction.Y < l && direction.Y >= 0)
                {
                    var state = CopyState(node.State);
                    state[zero.Y, zero.X] = state[direction.Y, direction.X];
                    state[direction.Y, direction.X] = 0;
                    neighbours.Add(
                        new Node(state,
                            new Point(direction.X, direction.Y)));
                }
	        }

            return neighbours; 
        }

        private static int[,] CopyState(int[,] state)
        {
            var l = state.GetLength(0);
            var copy = new int[l, l];

            for (int y = 0; y < l; y++)
            {
                for (int x = 0; x < l; x++)
                {
                    copy[y, x] = state[y, x];
                }
            }

            return copy;
        }

        private static KeyValuePair<int, Node> Pop(SortedList<int, Node> q)
        {
            var element = q.First();
            q.RemoveAt(0);
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
            var l = state.GetLength(0);

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

            return new Point();
        }
        
        private static IDictionary<int, Point> coordinates = new Dictionary<int, Point>();

        struct Point 
        {
            public Point(int x, int y)
                : this()
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

            public Point ZeroIndex { get; set; }

            public Node(int path, int heuristics, int[,] state)
                : this()
            {
                this.Path = path;
                this.Heuristics = heuristics;
                this.State = state;
            }

            public Node(int[,] state)
                : this()
            {
                this.State = state;
            }

            public Node(int[,] state, Point zeroIndex)
                : this()
            {
                this.State = state;
                this.ZeroIndex = zeroIndex;
            }
        }

        public class DuplicateKeyComparer<TKey>
                :IComparer<TKey> where TKey : IComparable
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

        public class StateEqualityComparer<TKey>
                : IEqualityComparer<TKey>
        {

            public bool Equals(TKey lft, TKey r)
            {
                var left = lft as int[,];
                var right = r as int[,];

                var l = left.GetLength(0);

                for (int y = 0; y < l; y++)
                {
                    for (int x = 0; x < l; x++)
                    {
                        if (left[y, x] != right[y, x]) return false;
                    }
                }

                return true;
            }

            public int GetHashCode(TKey obj)
            {
                var state = obj as int[,];
                var l = state.GetLength(0);
                var builder = new StringBuilder();
                for (int y = 0; y < l; y++)
                {
                    for (int x = 0; x < l; x++)
                    {
                        builder.Append(state[y, x]);
                    }
                }
                return builder.ToString().GetHashCode();
            }
        }
    }    
}
