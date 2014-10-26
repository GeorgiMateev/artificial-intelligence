using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Queens
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        private static void Place(int n)
        {
            // Contains the number of conflicts on every place of the board.
            var conflictsBoard = new int[n, n];
     
            // Contains the coordinates of the queen.
            var queensCoordinates = new Queen[n];

            PlaceRandom(queensCoordinates);

            IList<Queen> conflicting;
            do
            {
                UpdateConflictsBoard(conflictsBoard, queensCoordinates);

                conflicting = FindConflicting(queensCoordinates, conflictsBoard);

                var queen = ChooseRandom(conflicting);

                PlaceOnMinimumConflicts(queen, conflictsBoard);
            } 
            while (conflicting.Count != 0);
        }

        private static void UpdateConflictsBoard(int[,] conflictsBoard, Queen[] queens)
        {
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
            }

            queen.Y = minIndex;
        }

        private static Queen ChooseRandom(IList<Queen> conflicting)
        {
            var r = new Random();
            var index = r.Next(conflicting.Count - 1);
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

            // Mark that there is queen on this place. No other place can have such number of conflicts.
            conflictsBoard[queen.Y, queen.X] = l;

            var boundaryIndex = l - 1;
            for (int ofset = 0; ofset < l; ofset++)
            {

            }
        }

        private static void PlaceRandom(Queen[] queens)
        {
            var r = new Random();

            var l = queens.Length;
            for (int column = 0; column < l; column++)
            {
                var row = r.Next(l - 1);
                var q = new Queen(row, column);

                queens[column] = q;
            }
        }

        struct Queen
        {
            public Queen(int x, int y)
                :this()
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
