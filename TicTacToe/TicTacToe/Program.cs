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
            var state = new int[3, 3];
            int? result;
            do
            {
                HumanMove(state, Players.X);
                Tuple<int, int> bestMove;
                result = Min(int.MinValue, int.MaxValue, CopyState(state), out bestMove);
                state[bestMove.Item1, bestMove.Item2] = (int)Players.O;
            }
            while (result != 0 || result != (int)Players.O || result != (int)Players.X);

            Console.WriteLine("Result: " + result.Value);
        }

        private static int? Max(int alfa, int beta, int[,] state)
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

                Tuple<int, int> bestMove;
                var minLimit = Min(alfa, beta, state, out bestMove);

                // alfa = max(alfa, minLimit)
                if (minLimit.HasValue && alfa < minLimit.Value) alfa = minLimit.Value;

                if (alfa >= beta) break;

                // revert move
                state[move.Item1, move.Item2] = 0;
            }            

            return alfa;
        }                 

        private static int? Min(int alfa, int beta, int[,] state, out Tuple<int, int> bestMove)
        {
            bestMove = null;
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

                // beta = min(, maxLimit, beta)
                if (maxLimit.HasValue && maxLimit < beta) beta = maxLimit.Value;

                if (alfa >= beta)
                {
                    bestMove = Tuple.Create<int, int>(move.Item1, move.Item2);
                    break;
                };

                // revert move
                state[move.Item1, move.Item2] = 0;
            }

            return beta;
        }

        private static void HumanMove(int[,] state, Players player)
        {
            for (int r = 0; r < state.GetLength(0); r++)
            {
                for (int c = 0; c < state.GetLength(0); c++)
                {
                    Console.Write(((state[r, c] != 0) ? ((Players)state[r, c]).ToString() : "_") + " ");
                }
                Console.WriteLine();

            }
            Console.WriteLine();

            var move = Console.ReadLine().Split(',').Select(x => int.Parse(x)).ToList();
            state[move[0], move[1]] = (int)player;
        }     

        private static List<Tuple<int, int>> Moves(int[,] state)
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

        private static bool Evaluate(int[,] state, out int result)
        {
            result = 0;

            var length = state.GetLength(0);            

            // check horizontals
            for (int row = 0; row < length; row++)
            {
                var winningSymbol = state[row, 0];
                var haveWinner = true;
                for (int column = 0; column < length; column++)
                {
                    // there are empty places
                    if (state[row, column] == 0) return false;

                    if (state[row, column] != winningSymbol)
                    {
                        haveWinner = false;
                        break;
                    }                    
                }

                if (haveWinner) 
                {
                    result = (int)winningSymbol;
                    //for (int r = 0; r < state.GetLength(0); r++)
                    //{
                    //    for (int c = 0; c < state.GetLength(0); c++)
                    //    {
                    //        Console.Write(state[r, c] + " ");
                    //    }
                    //    Console.WriteLine();
                        
                    //}
                    //Console.WriteLine();
                    return true;
                }
            }

            // check vericals
            for (int column = 0; column < length; column++)
            {
                var winningSymbol = state[0, column];
                var haveWinner = true;

                for (int row = 0; row < length; row++) {
                    // there are empty places
                    if (state[row, column] == 0) return false;

                    if (state[row, column] != winningSymbol)
                    {
                        haveWinner = false;
                        break;
                    }                    
                }

                if (haveWinner)
                {
                    result = (int)winningSymbol;
                    //for (int r = 0; r < state.GetLength(0); r++)
                    //{
                    //    for (int c = 0; c < state.GetLength(0); c++)
                    //    {
                    //        Console.Write(state[r, c] + " ");
                    //    }
                    //    Console.WriteLine();
                        
                    //}
                    //Console.WriteLine();
                    return true;
                }
            }

            // draw
            return true;
        }

        private static int[,] CopyState(int[,] state)
        {            
            var length = state.GetLength(0);
            var copy = new int[length, length];

            for (int r = 0; r < length; r++)
            {
                for (int c = 0; c < length; c++)
                {
                    copy[r, c] = state[r, c];
                }
            }

            return copy;
        }

        private enum Players
        {
            X = int.MaxValue,
            O = int.MinValue
        }
    }
}
