using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SudokuSolver.Solver
{
    public static class Solver
    {
        /// <summary>
        /// Takes an array board state, representing a Sudoku puzzle state, and solve it, if possible.
        /// </summary>
        /// <param name="boardState">2D 9x9 sbyte[,] representing the board state, with 0's for unknown values.</param>
        /// <returns>2D 9x9 sbyte[,] representing a corresponding solution.</returns>
        public static sbyte[,] SolvePuzzle([NotNull] sbyte[,] boardState)
        {
            // Check for null board state
            if (boardState is null)
                throw new ArgumentNullException(nameof(boardState));

            // Create SudokuState object
            var state = new SudokuState(boardState);

            // Simple solve (deterministic, non branching)
            state.SimpleSolve();

            // Attempt a branching solution (can be CPUMemory intensive)
            if (BranchingSolve(ref state))
                return state.BoardState;

            // No solution?
            return null;
        }

        /// <summary>
        /// Takes a sudoku puzzle as an 81 character string, solves it, and returns a corresponding 81 character string solution.
        /// </summary>
        /// <param name="boardString">81-character string representing the board state, with 0's for unknown values</param>
        /// <returns>81-character string representing a corresponding solution to the passed in puzzle state</returns>
        public static string SolvePuzzle([NotNull] string boardString)
        {
            // Turn passed in string into a board (sbyte[9, 9])
            var boardState = DeserializeBoard(boardString);

            // Solve it with the other method
            var solution = SolvePuzzle(boardState);

            // Return serialized sulution
            return SerializeSudoku(solution) ?? string.Empty;
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

        // Serializes a Sudoku state into an 81 character string
        public static string SerializeSudoku([NotNull] SudokuState sudokuState)
        {
            if (sudokuState is null) throw new ArgumentNullException(nameof(sudokuState));
            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    sb.Append(sudokuState[i, j]);

            return sb.ToString();
        }

        // Serializes a board state into an 81 character string
        public static string SerializeSudoku([NotNull] sbyte[,] boardState)
        {
            if (boardState is null) return null;

            if (boardState.GetLength(0) != 9
                || boardState.GetLength(1) != 9)
                throw new ArgumentException(BadDimensions);

            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    sb.Append(boardState[i, j]);

            return sb.ToString();
        }

        // Deserializes an 81-character sudoku string into a board state array
        public static sbyte[,] DeserializeBoard([NotNull] string serBoardState)
        {
            if (string.IsNullOrEmpty(serBoardState))
                throw new ArgumentNullException(NullOrEmptyBoardState);
            
            if (serBoardState.Length != 81)
                throw new ArgumentException(BadDimensions);

            var cinv = CultureInfo.InvariantCulture;

            sbyte[,] buffer = new sbyte[9, 9];

            for (int i = 0; i < 9; i++) for (int j = 0; j < 9; j++)
                    buffer[i, j] = sbyte.Parse(serBoardState
                        .ElementAt(9 * i + j)
                        .ToString(cinv)
                        .Replace('.', '0'), cinv);

            return buffer;
        }

        private static string BadDimensions => "Invalid dimensions for sudoku puzzle.";
        private static string NullOrEmptyBoardState => "Cannot deserialize null or empty string.";
    }
}
