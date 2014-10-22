using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frogs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter N - the number of frogs watching in one direction.");
            var n = int.Parse(Console.ReadLine());

            var l = 2*n +1;
            var state = new int[l];
            for (int i = 0; i < n; i++)
            {
                state[i] = 1;
                state[l-i-1] = 2;
            }

            var p = new Program();
            p.Frogs(state);
        }

        private void Frogs(int[] state)
        {
            this.firstState = this.CopyState(state);
            this.path = new Stack<int[]>();

            int emptyIndex = 0;
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == 0)
                {
                    emptyIndex = i;
                }
            }

            this.Dfs(state, emptyIndex);
        }        

        private bool Dfs(int[] state, int emptyIndex)
        {
            this.path.Push(this.CopyState(state));

            if (this.isFinal(state))
            {
                this.printPath();
                return true;
            }

            var moves = new int[] { -1, -2, 1, 2 };
            foreach (var index in moves)
	        {
                int currentIndex = emptyIndex - index;
		        if (currentIndex  >= 0  && currentIndex < state.Length)
                {
                    if ((index < 0 && state[currentIndex] != 2) ||
                        (index > 0 && state[currentIndex] != 1))
                    {
                        continue;
                    }

                    state[emptyIndex] = state[currentIndex];
                    state[currentIndex] = 0;

                    var found = this.Dfs(state, currentIndex);
                    if (found) return true;

                    state[currentIndex] = state[emptyIndex];
                    state[emptyIndex] = 0;

                    this.path.Pop();
                }
	        }

            return false;
        }

        private void printPath()
        {
            while (this.path.Count > 0)
            {
                var state = this.path.Pop();
                foreach (var item in state)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine();
            }
        }

        private bool isFinal(int[] state)
        {
            bool isFinal = true;
            for (int i = 0; i < state.Length; i++)
            {
                switch (this.firstState[i])
                {
                    case 1:
                        if (state[i] != 2) isFinal = false;
                        break;
                    case 2:
                        if(state[i] != 1) isFinal = false;
                        break;
                    case 0:
                        if (state[i] != 0) isFinal = false;
                        break;
                    default:
                        break;
                }
                if (!isFinal) return false;
            }

            return isFinal;
        }

        private int[] CopyState(int[] state)
        {
            var copy = new int[state.Length];
            for (int i = 0; i < state.Length; i++)
            {
                copy[i] = state[i];
            }

            return copy;
        }

        private int[] firstState;
        private Stack<int[]> path;
    }
}
