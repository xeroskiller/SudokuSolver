using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class SudokuState : IEquatable<SudokuState>
    {
        // List of all valid values
        private static readonly sbyte[] _Valid_Values
            = new sbyte[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Values populated in a row, by row index
        private List<sbyte>[] Rows { get; set; } = new List<sbyte>[9];
        // Values populated in a column, by column index
        private List<sbyte>[] Columns { get; set; } = new List<sbyte>[9];
        // Values populated in a square, by square index
        private List<sbyte>[] Squares { get; set; } = new List<sbyte>[9];

        // Array corresponding to each individual element of the board, containing all possible solution values for that cell
        public sbyte[,][] SolutionSpace { get; private set; }
        // Current state of the board, as a 2D array
        public sbyte[,] BoardState { get; private set; }

        // Number of unsolved cells on the board
        public int UnsolvedCellCount => (from sbyte x in BoardState where x == 0 select 1).Count();
        // Sum of number of possible solutions for all possible cells
        public int Entropy => (from sbyte[] x in SolutionSpace select x.Length).Sum() - 81;
        // Boolean indicating whether this state is solved
        public bool IsSolved => UnsolvedCellCount == 0;
        // This happens to be the condition encountered when a branching step was invalid
        public bool IsUnsolvable => (from sbyte[] a in SolutionSpace where a.Length == 0 select 1).Any();

        public SudokuState([NotNull] sbyte[,] board)
        {
            // Set non-null
            BoardState = board ?? throw new ArgumentNullException(nameof(board));

            // Initialize conjunctive element trackers
            for (int i = 0; i < 9; i++) // rows
            {
                Rows[i] = new List<sbyte>(9);
                Columns[i] = new List<sbyte>(9);
                Squares[i] = new List<sbyte>(9);
            }

            for (int i = 0; i < 9; i++) // rows
            {
                for (int j = 0; j < 9; j++) // cols
                {
                    // Get square indices
                    var sqIdx = SqIdxs(i, j);

                    // Set row, col, and square conjunctive values
                    if (BoardState[i, j] != 0)
                    {
                        Rows[i].Add(BoardState[i, j]);
                        Columns[j].Add(BoardState[i, j]);
                        Squares[sqIdx].Add(BoardState[i, j]);
                    }
                }
            }

            // Calculate possible solutions, per cell
            CalculateSolutionSpace();
        }

        public SudokuState Clone()
            => new SudokuState(BoardState.Clone() as sbyte[,]);

        public static bool IsValidBoardState(string boardState)
            => IsValidBoardState(Solver.DeserializeBoard(boardState));

        /// <summary>
        /// Validates a board state for sudoku and returns a boolean value indicating its validity
        /// </summary>
        /// <param name="boardState">2D 9x9 sbyte[,] representing board state</param>
        /// <returns>Boolean value indicating if the passed in board state is valid</returns>
        public static bool IsValidBoardState(sbyte[,] boardState)
        {
            if (boardState is null) return false;

            // BoardState dimensions
            if (boardState.GetLength(0) != 9
                || boardState.GetLength(1) != 9)
                return false;

            // Values
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (boardState[i, j] < 0 || boardState[i, j] > 9) 
                        return false;

            // Rows
            for (int i = 0; i < 9; i++)
            {
                sbyte[] buffer = new sbyte[9];

                for (int j = 0; j < 9; j++)
                    if (boardState[i, j] != 0)
                        buffer[boardState[i, j] - 1]++;

                if (buffer.Any(x => x > 1)) return false;
            }

            // Cols
            for (int y = 0; y < 9; y++)
            {
                sbyte[] buffer = new sbyte[9];

                for (int x = 0; x < 9; x++)
                    if (boardState[x, y] != 0)
                        buffer[boardState[x, y] - 1]++;

                if (buffer.Any(x => x > 1)) return false;
            }

            // Squares
            for (int i = 0; i < 9; i++)
            {
                sbyte[] buffer = new sbyte[9];

                for (int j = 0; j < 9; j++)
                    if (boardState[3 * (i / 3), j] != 0)
                        buffer[boardState[, ] - 1]++;

                if (buffer.Any(c => c > 1)) return false;
            }

            return true;
        }

        // recalculates the solution space, based on the current board state
        private void CalculateSolutionSpace()
        {
            if (SolutionSpace is null)
                SolutionSpace = new sbyte[9, 9][];

            for (int i = 0; i < 9; i++) // Rows
            {
                for (int j = 0; j < 9; j++) // Cols
                {
                    if (BoardState[i, j] != 0)
                        SolutionSpace[i, j] = new sbyte[1] { BoardState[i, j] };
                    else
                    {
                        // Get square index
                        sbyte sqIdx = SqIdxs(i, j);

                        // Get listing of all distinct conjunctive elements
                        var conjunctiveElements = Rows[i].Union(Columns[j]).Union(Squares[sqIdx]).Distinct();

                        // Set solution space as collection of all values minus conjunctive values
                        SolutionSpace[i, j] = _Valid_Values.Except(conjunctiveElements).ToArray();
                    }
                }
            }
        }

        // Attempts a simple, deterministic solution. Cells with only one possible solution
        //      are assigned, and the solution space is recalculated, until there are not more
        //      solved cells, or the puzzle is solved.
        internal bool SimpleSolve()
        {
            (int row, int col, sbyte value) solvedCell;

            do
            {
                solvedCell = GetSolvedCell();

                if (solvedCell == (-1, -1, -1)) break;
            } while (SetCellValue(solvedCell.row, solvedCell.col, solvedCell.value));

            return IsSolved;
        }

        /// <summary>
        /// Set the value of a particular cell, then recalculate the solution space.
        /// </summary>
        /// <param name="solvedCell">Tuple indicating coordinates and value of cell to be assigned</param>
        /// <returns>bool indicating success condition</returns>
        public bool SetCellValue(ValueTuple<int, int, sbyte> solvedCell)
            => SetCellValue(solvedCell.Item1, solvedCell.Item2, solvedCell.Item3);

        /// <summary>
        /// Set the value of a particular cell, then recalculate the solution space.
        /// </summary>
        /// <param name="row">row index of cell to be set</param>
        /// <param name="col">column index of cell to be set</param>
        /// <param name="value">value to set cell to</param>
        /// <returns>bool indicating success condition</returns>
        public bool SetCellValue(int row, int col, sbyte value)
        {
            // Check values and ensure theyre reasonable
            if (row < 0 || row >= 9) return false;
            if (col < 0 || col >= 9) return false;
            if (value < 1 || value > 9) return false;

            // Get dims for square conversion
            sbyte sqIdx = SqIdxs(row, col);

            // Fail if this value already exists in a conjunctive element
            if (Rows[row].Contains(value)) return false;
            if (Columns[col].Contains(value)) return false;
            if (Squares[sqIdx].Contains(value)) return false;

            // Set value and solution space set
            BoardState[row, col] = value;

            // Add value to appropriate conjunctive element classes
            Rows[row].Add(value);
            Columns[col].Add(value);
            Squares[sqIdx].Add(value);

            CalculateSolutionSpace();

            // return true
            return true;
        }

        // Returns enumerable of all cells with solutions available
        internal (int row, int col, sbyte value) GetSolvedCell()
        {
            for (int i = 0; i < 9; i++) // Rows
                for (int j = 0; j < 9; j++) // Cols
                    if (SolutionSpace[i, j].Length == 1 && BoardState[i, j] == 0)
                        return (i, j, SolutionSpace[i, j][0]);

            return (-1, -1, -1);
        }

        internal (int row, int col, sbyte value)[] GetAllSolvedCells()
        {
            var results = new List<(int, int, sbyte)>();

            for (int i = 0; i < 9; i++) // Rows
                for (int j = 0; j < 9; j++) // Cols
                    if (SolutionSpace[i, j].Length == 1 && BoardState[i, j] == 0)
                        results.Add((i, j, SolutionSpace[i, j][0]));

            return results.ToArray();
        }

        protected internal (int row, int col, sbyte[] vals)? GetKeystone()
        {
            // Buffer for keystone
            (int row, int col, sbyte val)? uncCoords = null;

            // Loop over all cells, storing as keystone if Uncertainty is less
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (BoardState[i, j] == 0                                       // Cell must not have a value to be a keystone
                        && SolutionSpace[i, j].Length > 1                           // SolutionSpace must have multiple options
                        && (!uncCoords.HasValue                                     // First cell meeting these conditions?
                            || SolutionSpace[i, j].Length < uncCoords.Value.val))   // Not the first, but less uncertainty -> smaller tree
                        uncCoords = (i, j, (sbyte)SolutionSpace[i, j].Length);

            // Return the appropriate values
            return uncCoords is null 
                ? (ValueTuple<int, int, sbyte[]>?)null 
                : (uncCoords.Value.row, uncCoords.Value.col, SolutionSpace[uncCoords.Value.row, uncCoords.Value.col]);
        }

        // Shortcut for indexing to get board state value
        public sbyte this[int row, int col] => BoardState[row, col];

        // Equality is based strictly on the board state values
        public bool Equals([AllowNull] SudokuState other)
        {
            if (this == null && other == null)
                return true;
            if (this != null && other != null
                && GetHashCode().Equals(other.GetHashCode()))
                return true;
            return false;
        }

        // Equality is based strictly on the board state values
        public override bool Equals(object obj)
        {
            return Equals(obj as SudokuState);
        }

        // Based entirely on current board state values
        public override int GetHashCode() => BoardState.GetHashCode();

        #region Square Trickery
        // This is necessary to ease retrieving the conjunctive values from a cells
        //      3x3 neighborhood
        private static readonly sbyte[,] _Square_Index_Mapping
            = new sbyte[9, 9]
            {
                { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
                { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
                { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
                { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
                { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
                { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
                { 6, 6, 6, 7, 7, 7, 8, 8, 8 },
                { 6, 6, 6, 7, 7, 7, 8, 8, 8 },
                { 6, 6, 6, 7, 7, 7, 8, 8, 8 },
            };
        private static sbyte SqIdxs(int row, int col)
            => _Square_Index_Mapping[row, col];
        #endregion
    }
}
