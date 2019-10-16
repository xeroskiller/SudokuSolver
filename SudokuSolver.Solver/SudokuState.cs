using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class SudokuState : IEquatable<SudokuState>, IComparable<SudokuState>
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
        public int Uncertainty => (from sbyte[] x in SolutionSpace select x.Length).Sum();
        // Boolean indicating whether this state is solved
        public bool IsSolved => UnsolvedCellCount == 0;
        public string Ser => string.Join(string.Empty, from sbyte m in BoardState select m);
        public bool IsUnsolvable
            => (from sbyte[] a in SolutionSpace where a.Length == 0 select 1).Any();

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

        public bool SimpleSolve()
        {
            (int row, int col, sbyte value) solvedCell;

            do
            {
                solvedCell = GetSolvedCell();

                if (solvedCell == (-1, -1, -1)) break;
            } while (SetCellValue(solvedCell.row, solvedCell.col, solvedCell.value));

            return IsSolved;
        }

        public bool SetCellValue(ValueTuple<int, int, sbyte> solvedCell)
            => SetCellValue(solvedCell.Item1, solvedCell.Item2, solvedCell.Item3);
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

            // Add  value to appropriate conjunctive element classes
            Rows[row].Add(value);
            Columns[col].Add(value);
            Squares[sqIdx].Add(value);

            CalculateSolutionSpace();

            // return true
            return true;
        }

        // Returns enumerable of all cells with solutions available
        public (int row, int col, sbyte value) GetSolvedCell()
        {
            for (int i = 0; i < 9; i++) // Rows
                for (int j = 0; j < 9; j++) // Cols
                    if (SolutionSpace[i, j].Length == 1 && BoardState[i, j] == 0)
                        return (i, j, SolutionSpace[i, j][0]);

            return (-1, -1, -1);
        }

        public (int row, int col, sbyte value)[] GetAllSolvedCells()
        {
            var results = new List<(int, int, sbyte)>();

            for (int i = 0; i < 9; i++) // Rows
                for (int j = 0; j < 9; j++) // Cols
                    if (SolutionSpace[i, j].Length == 1 && BoardState[i, j] == 0)
                        results.Add((i, j, SolutionSpace[i, j][0]));

            return results.ToArray();
        }

        public (int row, int col, sbyte[] vals)? GetKeystone()
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

        public sbyte this[int row, int col] => BoardState[row, col];

        #region Square Trickery
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

        #region IEquatable and IComparable
        public int CompareTo([AllowNull] SudokuState other)
            => (this) == null 
                ? -1
                : other != null 
                    ? UnsolvedCellCount.CompareTo(other.UnsolvedCellCount) 
                    : 1;

        public bool Equals([AllowNull] SudokuState other)
        {
            if (this == null && other == null)
                return true;
            if (this != null && other != null
                && GetHashCode().Equals(other.GetHashCode()))
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SudokuState);
        }

        public override int GetHashCode() => BoardState.GetHashCode();

        //public static bool operator ==(SudokuState left, SudokuState right)
        //{
        //    if (left is null)
        //        return right is null;
        //    return left.Equals(right);
        //}

        //public static bool operator !=(SudokuState left, SudokuState right)
        //{
        //    return !(left == right);
        //}

        //public static bool operator <(SudokuState left, SudokuState right)
        //{
        //    return left is null ? right is object : left.CompareTo(right) < 0;
        //}

        //public static bool operator <=(SudokuState left, SudokuState right)
        //{
        //    return left is null || left.CompareTo(right) <= 0;
        //}

        //public static bool operator >(SudokuState left, SudokuState right)
        //{
        //    return left is object && left.CompareTo(right) > 0;
        //}

        //public static bool operator >=(SudokuState left, SudokuState right)
        //{
        //    return left is null ? right is null : left.CompareTo(right) >= 0;
        //}
        #endregion
    }
}
