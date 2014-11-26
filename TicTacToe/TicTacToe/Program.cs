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
            if (true)
            {
                var state = new int[3, 3];
                int? result;
                while(true)
                {
                    HumanMove(state, Players.X);
                    Tuple<int, int> bestMove;
                    result = Min(-10, 10, CopyState(state), out bestMove);

                    if (GameCompleted(result, state)) break;

                    state[bestMove.Item1, bestMove.Item2] = (int)Players.O;
                }

                Console.WriteLine("The winner is: {0}", ((Players)result).ToString());
            }
            else
            {
                var state = new int[,]
                {
                    {(int)Players.N, (int)Players.N, (int)Players.N},
                    {(int)Players.N, (int)Players.N, (int)Players.N},
                    {(int)Players.X, (int)Players.N, (int)Players.N}
                };
                Tuple<int, int> best;
                int? result = Min(-10, 10, state, out best);
                Console.WriteLine("The winner is: {0}", ((Players)result).ToString());
            }            
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

                Tuple<int, int> bestMove = null;
                var minLimit = Min(alfa, beta, state, out bestMove);

                // alfa = max(alfa, minLimit)
                if (minLimit.HasValue && alfa < minLimit.Value) alfa = minLimit.Value;

                // revert move
                state[move.Item1, move.Item2] = 0;

                if (alfa > beta) break;                
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
            var minBeta = 10;

            foreach (var move in moves)
            {
                // do move
                state[move.Item1, move.Item2] =(int) Players.O;

                var maxLimit = Max(alfa, beta, state);

                // beta = min(, maxLimit, beta)
                if (maxLimit.HasValue && maxLimit < beta) beta = maxLimit.Value;

                // remember the move with the smallest beta
                if (beta < minBeta || bestMove == null)
                {
                    bestMove = move;
                    minBeta = beta;
                }

                // revert move
                state[move.Item1, move.Item2] = 0;

                if (alfa > beta)
                {
                    break;
                };                
            }

            return beta;
        }

        private static void HumanMove(int[,] state, Players player)
        {
            Console.WriteLine();
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

            if (state[move[0], move[1]] != (int)Players.N)
            {
                Console.WriteLine("Invalid move!");
                HumanMove(state, player);
            }
            {
                state[move[0], move[1]] = (int)player;
            }
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
            var isFinal = true;
            var haveWinner = true;
            var winningSymbol = 0;

            // check horizontals
            for (int row = 0; row < length; row++)
            {
                winningSymbol = state[row, 0];
                haveWinner = true;
                for (int column = 0; column < length; column++)
                {
                    // there are empty places
                    if (state[row, column] == 0)
                    {
                        haveWinner = false;
                        isFinal = false;
                        break;
                    } 

                    if (state[row, column] != winningSymbol)
                    {
                        haveWinner = false;
                        break;
                    }                    
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
                winningSymbol = state[0, column];
                haveWinner = true;

                for (int row = 0; row < length; row++) {
                    // there are empty places
                    if (state[row, column] == 0)
                    {
                        haveWinner = false;
                        isFinal = false;
                        break;
                    } 

                    if (state[row, column] != winningSymbol)
                    {
                        haveWinner = false;
                        break;
                    }                    
                }

                if (haveWinner)
                {
                    result = (int)winningSymbol;
                    return true;
                }
            }

            haveWinner = true;
            winningSymbol = state[0, 0];
            for (int i = 0; i < length; i++)
            {
                if (state[i, i] != winningSymbol)
                {
                    haveWinner = false;
                    break;
                }

                if (state[i, i] == 0)
                {
                    isFinal = false;
                    haveWinner = false;
                    break;
                }
            }

            haveWinner = true;
            winningSymbol = state[0, length - 1];
            for (int i = 0; i < length; i++)
            {
                if (state[i, length - 1 - i] != winningSymbol)
                {
                    haveWinner = false;
                    break;
                }

                if (state[i, length - 1 - i] == 0)
                {
                    isFinal = false;
                    haveWinner = false;
                    break;
                }
            }

            if (haveWinner)
            {
                result = (int)winningSymbol;
                return true;
            }

            // draw if true
            return isFinal;
        }

        private static bool GameCompleted(int? result, int[,] state)
        {
            if (result == (int)Players.O || result == (int)Players.X)
                return true;

            return result == 0 && IsFilled(state);
        }

        private static bool IsFilled(int[,] state)
        {
            var length = state.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (state[i, j] == 0) return false;
                }
            }
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
            X = 10,
            O = -10,
            N = 0
        }
    }
}
