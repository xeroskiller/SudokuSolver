using SudokuSolver.Solver.Implementations.Internal;
using SudokuSolver.Solver.Interfaces;
using System;

namespace SudokuSolver.Solver.Implementations
{
    public class BranchingSudokuSolver : ISudokuSolver
    {
        public BranchingSudokuSolver() { }

        public sbyte[,] SolvePuzzle(sbyte[,] boardState)
        {
            // Check for null board state
            if (boardState is null)
                throw new ArgumentNullException(nameof(boardState));

            // Validate the board state
            if (!SudokuState.IsValidBoardState(boardState))
                throw new ArgumentException(InvalidBoardState);

            var state = new SudokuState(boardState);

            return SolvePuzzle_Internal(state);
        }

        public string SolvePuzzle(string boardState)
        {
            // Check for null board state
            if (boardState is null)
                throw new ArgumentNullException(nameof(boardState));

            var state = new SudokuState(boardState.DeserializeBoard());

            return SolvePuzzle_Internal(state).SerializeSudoku();
        }

        /// <summary>
        /// Takes an array board state, representing a Sudoku puzzle state, and solve it, if possible.
        /// </summary>
        /// <param name="boardState">2D 9x9 sbyte[,] representing the board state, with 0's for unknown values.</param>
        /// <returns>2D 9x9 sbyte[,] representing a corresponding solution.</returns>
        internal sbyte[,] SolvePuzzle_Internal(SudokuState state)
        {
            // Simple solve (deterministic, non branching)
            state.SimpleSolve();
            if (state.IsSolved) return state.BoardState;

            // Attempt a branching solution (can be CPUMemory intensive)
            if (BranchingSolve(ref state))
                return state.BoardState;

            // No solution?
            return null;
        }

        // Attempts a branching solution, by randomly assigning an unsolved cell of lowest entropy
        //      And then attempting a simple solve. Continues to branch and prune unsolvables until
        //      it solves it, or runs out of memory.
        private static bool BranchingSolve(ref SudokuState state)
        {
            // Buffer for solution state
            SudokuState solution;

            // Use a state tree to do the branching
            using (var solver = new SudokuStateTree(state))
                solution = solver.Solve();     

            // If it returned a solution, set it
            if (solution != null) state = solution;

            // Return boolean indicating whether this was successful in finding a solution
            return solution != null;
        }

        private string InvalidBoardState => "Invalid board state.";
    }
}
