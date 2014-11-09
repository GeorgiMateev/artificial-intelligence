using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Queens
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of queens: ");
            var n = int.Parse(Console.ReadLine());
            var queens = Place(n);

            var board = new int[n, n];

            foreach (var q in queens)
            {
                board[q.Y, q.X] = 1;
            }

            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    if (board[y, x] == 1)
                    {
                        Console.Write("X ");
                    }
                    else
                    {
                        Console.Write("_ ");
                    }
                }
                Console.WriteLine();
            }
        }

        private static Queen[] Place(int n)
        {
            // Contains the number of conflicts on every place of the board.
            var conflictsBoard = new int[n, n];
     
            // Contains the coordinates of the queen.
            var queensCoordinates = new Queen[n];

            PlaceRandom(queensCoordinates);

            IList<Queen> conflicting;

            while(true)
            {
                UpdateConflictsBoard(conflictsBoard, queensCoordinates);

                conflicting = FindConflicting(queensCoordinates, conflictsBoard);

                if (conflicting.Count == 0) 
                    break;

                var queen = ChooseRandom(conflicting);

                PlaceOnMinimumConflicts(queen, conflictsBoard);
            }

            return queensCoordinates;
        }

        private static void UpdateConflictsBoard(int[,] conflictsBoard, Queen[] queens)
        {
            // Clean the board first
            var l = conflictsBoard.GetLength(0);
            for (int y = 0; y < l; y++)
            {
                for (int x = 0; x < l; x++)
                {
                    conflictsBoard[y, x] = 0;
                }
            }

            for (var i = 0; i < queens.Length; i++)
            {
                var queen = queens[i];

                TraverseBoard(queen, conflictsBoard);
            }
        }

        private static void PlaceOnMinimumConflicts(Queen queen, int[,] conflictingBoard)
        {
            var l = conflictingBoard.GetLength(0);
            var column = queen.X;
            var minIndex = 0;
            var min = l;

            for (int row = 0; row < l; row++)
            {
                var conflicts = conflictingBoard[row, column];
                if (conflicts < min)
                {
                    min = conflicts;
                    minIndex = row;
                }
                else if(conflicts == min && row != queen.Y)
                {
                    // Move the queen if she is already in a place with min conflicts to avoid her staying there forever.
                    minIndex = row;
                }
            }

            queen.Y = minIndex;
        }

        private static Queen ChooseRandom(IList<Queen> conflicting)
        {
            var r = new Random();
            var index = r.Next(conflicting.Count);
            return conflicting[index];
        }

        private static IList<Queen> FindConflicting(Queen[] queensCoordinates, int[,] conflictingBoard)
        {
            var conflicts = new List<Queen>();
            foreach (var queen in queensCoordinates)
            {
                if (conflictingBoard[queen.Y, queen.X] > 0)
                {
                    conflicts.Add(queen);
                }
            }

            return conflicts;
        }

        private static void TraverseBoard(Queen queen, int[,] conflictsBoard)
        {
            var l = conflictsBoard.GetLength(0);

            for (int ofset = 1; ofset < l; ofset++)
            {
                // horizontal
                UpdateField(queen.X + ofset, queen.Y, conflictsBoard);
                UpdateField(queen.X - ofset, queen.Y, conflictsBoard);

                // left diagonal
                UpdateField(queen.X - ofset, queen.Y - ofset, conflictsBoard);
                UpdateField(queen.X + ofset, queen.Y + ofset, conflictsBoard);

                // right diagonal
                UpdateField(queen.X + ofset, queen.Y - ofset, conflictsBoard);
                UpdateField(queen.X - ofset, queen.Y + ofset, conflictsBoard);
            }
        }

        private static void UpdateField(int x, int y, int[,] board)
        {
            var l = board.GetLength(0);
            var boundaryIndex = l - 1;

            if (x >= 0 && y >= 0 &&
                x <= boundaryIndex && y <= boundaryIndex)
            {
                board[y, x] += 1;
            }
        }

        private static void PlaceRandom(Queen[] queens)
        {
            var r = new Random();

            var l = queens.Length;
            for (int column = 0; column < l; column++)
            {
                var row = r.Next(l);
                var q = new Queen(column, row);

                queens[column] = q;
            }
        }

        [DebuggerDisplay("X: {X}, Y: {Y}")]
        class Queen
        {
            public Queen(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
            public bool InConflict { get; set; }
        }
    }
}
