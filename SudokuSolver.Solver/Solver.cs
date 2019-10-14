using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class SudokuSolved
    {
        public SudokuState InitialState { get; }
        private bool solved = false;
        public bool Solved { get => solved; }
        public SudokuState SolvedState { get; private set; }

        public SudokuSolved(sbyte[,] boardState) => InitialState = new SudokuState(boardState);

        public void Solve(bool useBranchingAlgorithms = true)
        {
            if (Solved) return;

            int solvedCount;

            SudokuState workingState = InitialState;
            SudokuSolutionSpace space;

            do
            {
                space = new SudokuSolutionSpace(workingState);

                solvedCount = space.SolvedCells.Count;

                workingState = space.SolveAll();
            } while (solvedCount > 0);

            if (space.State.IsSolved)
            {
                solved = true;
                SolvedState = space.State;
                return;
            }

            if (useBranchingAlgorithms)
            {
                space = new SudokuSolutionSpace(workingState);

                var treeSolver = new SudokuStateTreeSolver(space);

                SolvedState = treeSolver.ProposeSolution();
            }
        }
    }

    public static class Extensions
    {
        public static IEnumerable<(T1, T2)> Cross<T1, T2>(this IEnumerable<T1> src, IEnumerable<T2> other)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (other == null) throw new ArgumentNullException(nameof(other));
            foreach (var a in src) foreach (var b in other) yield return (a, b);
        }
    }
}
