using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        private int Max(int alfa, int beta, int[,] state)
        {
            int result;
            if (Evaluate(state, out result))
            {
                return result;
            }

            var moves = Moves(state);

            foreach (var move in moves)
            {
                // do move
                state[move.Item1, move.Item2] = (int)Players.X;

                var minLimit = Min(alfa, beta, state);

                if (minLimit > alfa) alfa = minLimit;

                if (alfa >= beta) return alfa;

                // revert move
                state[move.Item1, move.Item2] = 0;
            }

            return (int)Players.X;
        }              

        private int Min(int alfa, int beta, int[,] state)
        {
            int result;
            if (Evaluate(state, out result))
            {
                return result;
            }

            var moves = Moves(state);

            foreach (var move in moves)
            {
                // do move
                state[move.Item1, move.Item2] =(int) Players.O;

                var maxLimit = Max(alfa, beta, state);

                if (maxLimit < alfa) alfa = maxLimit;

                if (alfa >= beta) return beta;

                // revert move
                state[move.Item1, move.Item2] = 0;
            }

            return (int)Players.O;
        }

        private List<Tuple<int, int>> Moves(int[,] state)
        {
            var moves = new List<Tuple<int, int>>();
            var length = state.GetLength(0);

            for (int row = 0; row < length; row++)
            {
                for (int column = 0; column < length; column++)
                {
                    if (state[row, column] == 0)
                    {
                        moves.Add(Tuple.Create<int, int>(row, column));
                    }
                }
            }

            return moves;
        }

        private bool Evaluate(int[,] state, out int result)
        {
            result = 0;

            var length = state.GetLength(0);            

            // check horizontals
            for (int row = 0; row < length; row++)
            {
                var winningSymbol = state[row, 0];
                var haveWinner = true;
                for (int column = 1; column < length; column++)
                {
                    if (state[row, column] != winningSymbol)
                    {
                        haveWinner = false;
                        break;
                    }

                    // there are empty places
                    if (state[row, column] == 0) return false;
                }

                if (haveWinner) 
                {
                    result = (int)winningSymbol;
                    return true;
                }
            }

            // check vericals
            for (int column = 0; column < length; column++)
            {
                var winningSymbol = state[0, column];
                var haveWinner = true;

                for (int row = 1; row < length; row++)
                {
                    if (state[row, column] != winningSymbol)
                    {
                        haveWinner = false;
                        break;
                    }

                    // there are empty places
                    if (state[row, column] == 0) return false;
                }

                if (haveWinner)
                {
                    result = (int)winningSymbol;
                    return true;
                }
            }

            // draw
            result = 0;
            return true;
        }


        private enum Players
        {
            X = 1,
            O = -1
        }
    }
}
