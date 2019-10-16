using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace SudokuSolver.Solver
{
    public static class Solver
    {
        private const int _Max_Branching_Depth = 20;

        public static sbyte[,] SolvePuzzle([NotNull] sbyte[,] boardState)
        {
            if (boardState is null)
                throw new ArgumentNullException(nameof(boardState));

            var state = new SudokuState(boardState);

            if (SimpleSolve(ref state))
                return state.BoardState;

            if (BranchingSolve3(ref state))
                return state.BoardState;

            Console.WriteLine();

            return null;
        }

        public static string SolvePuzzle([NotNull] string boardString)
        {
            var boardState = DeserializeBoard(boardString);

            var solution = SolvePuzzle(boardState);

            return SerializeSudoku(solution);
        }

        private static bool BranchingSolve(ref SudokuState state)
        {
            for (int i = 0; i < _Max_Branching_Depth; i++)
            {
                var workingState = state.Clone();
                var buffer = workingState.GetKeystone();

                (int row, int col, sbyte[] vals) x;

                if (buffer.HasValue) x = buffer.Value;
                else throw new Exception();

                var children = new SudokuState[x.vals.Length];

                for (int j = 0; j < x.vals.Length; j++)
                {
                    workingState.SetCellValue(x.row, x.col, x.vals[j]);

                    if (SimpleSolve(ref workingState)) return true;

                    children[j] = workingState;
                }

                var bestChild = children.OrderBy(child => child.UnsolvedCellCount).ThenBy(child => child.Uncertainty).First();

                if (bestChild.UnsolvedCellCount < state.UnsolvedCellCount) state = bestChild;
                else return false;
            }

            return false;
        }

        private static bool BranchingSolve2(ref SudokuState state)
        {
            var tree = new SudokuStateTreeNode(state);

            var solution = tree.TryGetSolution();

            if (solution != null)
            {
                state = solution;
                return true;
            }

            return false;
        }

        private static bool BranchingSolve3(ref SudokuState state)
        {
            SudokuState solution;

            using (var solver = new SudokuStateTree(state))
                solution = solver.Solve();     

            if (solution != null) state = solution;

            return solution != null;
        }

        private static bool SimpleSolve(ref SudokuState state)
        {
            (int row, int col, sbyte value) solvedCell;

            do
            {
                solvedCell = state.GetSolvedCell();

                if (solvedCell == (-1, -1, -1)) break;
            } while (state.SetCellValue(solvedCell.row, solvedCell.col, solvedCell.value)); 

            return state.IsSolved;
        }

        public static string SerializeSudoku([NotNull] SudokuState sudokuState)
        {
            if (sudokuState is null) throw new ArgumentNullException(nameof(sudokuState));
            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    sb.Append(sudokuState[i, j]);

            return sb.ToString();
        }

        public static string SerializeSudoku([NotNull] sbyte[,] boardState)
        {
            if (boardState is null) throw new ArgumentNullException(nameof(boardState));

            if (boardState.GetLength(0) != 9
                || boardState.GetLength(1) != 9)
                throw new ArgumentException(BadDimensions);

            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    sb.Append(boardState[i, j]);

            return sb.ToString();
        }

        public static sbyte[,] DeserializeBoard([NotNull] string serBoardState)
        {
            if (string.IsNullOrEmpty(serBoardState))
                throw new ArgumentNullException(NullOrEmptyBoardState);
            
            if (serBoardState.Length != 81)
                throw new ArgumentException(BadDimensions);

            sbyte[,] buffer = new sbyte[9, 9];

            for (int i = 0; i < 9; i++) for (int j = 0; j < 9; j++)
                    buffer[i, j] = sbyte.Parse(serBoardState.ElementAt(9 * i + j).ToString().Replace('.', '0'));

            return buffer;
        }

        private static string BadDimensions => "Invalid dimensions for sudoku puzzle.";
        private static string NullOrEmptyBoardState => "Cannot deserialize null or empty string.";
        private static string CatastrophicFailure => "Catastrophic failure.";
    }
}
