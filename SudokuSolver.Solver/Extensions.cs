using SudokuSolver.Solver.Implementations.Internal;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SudokuSolver.Solver
{
    public static class Extensions
    {
        // Serializes a Sudoku state into an 81 character string
        internal static string SerializeSudoku([NotNull] this SudokuState sudokuState)
        {
            if (sudokuState is null) throw new ArgumentNullException(nameof(sudokuState));
            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    sb.Append(sudokuState[i, j]);

            return sb.ToString();
        }

        // Serializes a board state into an 81 character string
        public static string SerializeSudoku([NotNull] this sbyte[,] boardState)
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
        public static sbyte[,] DeserializeBoard([NotNull] this string serBoardState)
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
