using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class Solver2
    {
        public SudokuState InitialState { get; }
        private bool solved1 = false;
        public bool Solved { get => Solved1; private set => Solved1 = value; }
        public SudokuState SolvedState { get; }
        public bool Solved1 { get => solved1; set => solved1 = value; }

        public Solver2(sbyte[,] boardState) => InitialState = new SudokuState(boardState);

        public bool Solve(bool useBranchingAlgorithms = true)
        {
            if (Solved1) return true;

            int solvedCount;
            SudokuState workingState = InitialState;

            SudokuSolutionSpace space;

            do
            {
                space = new SudokuSolutionSpace(workingState);

                solvedCount = space.SolvedCells.Count;

                foreach (var (row, col, val) in space.SolvedCells)
                    workingState.SetElement(row, col, val); 
            } while (solvedCount > 0);

            if (useBranchingAlgorithms)
            {
                var minUncertainty = Enumerable.Range(0, 9)
                    .Cross(Enumerable.Range(0, 9))
                    .Select(tpl => space.solutionSpace.Length).Min();

                var minUncertaintyItems = Enumerable.Range(0, 9)
                    .Cross(Enumerable.Range(0, 9))
                    .Select(tpl => (row: tpl.Item1, col: tpl.Item2, sln: space.solutionSpace[tpl.Item1, tpl.Item2]))
                    .Where(item => item.sln.Length == minUncertainty);

                var possibleSolutions = new List<SudokuState>();

                foreach (var tpl in minUncertaintyItems)
                {
                    foreach (var value in tpl.sln)
                    {
                        var buffer = workingState.BoardState;
                        buffer[tpl.row, tpl.col] = value;

                        var state = new SudokuState(buffer);
                        SudokuSolutionSpace solution;

                        do
                        {
                            solution = new SudokuSolutionSpace(state);

                            solvedCount = solution.SolvedCells.Count;

                            foreach (var (row, col, val) in solution.SolvedCells)
                                workingState.SetElement(row, col, val);
                        } while (solvedCount > 0);

                        possibleSolutions.Add(state);
                    }
                }
            }

            return true;
        }
    }
}
