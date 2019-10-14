﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class SudokuState
    {
        public sbyte[,] BoardState { get; set; }
        public bool IsSolved
            => !(from sbyte item in BoardState
                where item == 0
                select 1).Any();

        public string Serialized
            => string.Join(string.Empty, (from sbyte item in BoardState select item));

        public SudokuState(sbyte[,] boardState)
        {
            // Non null state
            if (boardState == null) 
                throw new ArgumentNullException(nameof(boardState));

            // Set the state
            BoardState = boardState;            

            // Validate values and dimensions
            ValidateBoardState(boardState);
        }

        // Clone to prevent ref issues
        public SudokuState Clone() => new SudokuState(BoardState.Clone() as sbyte[,]);

        // Throws when invalid.
        private bool ValidateBoardState(sbyte[,] boardState)
        { 
            if (boardState.GetLength(0) != 9
                || boardState.GetLength(1) != 9)
                throw new ArgumentException(Properties.Resources.GridDimensionExceptionMessage);

            var valid = new[] { (int)1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (!valid.Contains(boardState[i, j]))
                        throw new ArgumentException($"Cell countains value outside allowed range: ({j}, {i}): {boardState[i, j]}");

            // Rows must only contain one instance, at most, of each number in (1..9)
            for (int i = 0; i < 9; i++)
            {
                var x = GetRowElements(i);

                var y = (from j in x
                         where j != 0
                         group j by j into grp
                         select grp.Count()).Any(c => c > 1);

                if (y) throw new ArgumentException(Properties.Resources.StateValidationRowDegeneracyException);
            }

            // Columns must contain at most one instance of each number in (1..9)
            for (int i = 0; i < 9; i++)
            {
                var x = GetColumnElements(i);

                var y = (from j in x
                         where j != 0
                         group j by j into grp
                         select grp.Count()).Any(c => c > 1);

                if (y) throw new ArgumentException(Properties.Resources.StateValidationColumnDegeneracyException);
            }

            // Each 3x3 square s.t. the board is a 3x3 square of them must contain
            //      at most one instance of each number in (1..9)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var x = GetSquareElements(i, j);

                    var y = (from k in x
                             where k != 0
                             group k by k into grp
                             select grp.Count()).Any(c => c > 1);

                    if (y) throw new ArgumentException(Properties.Resources.StateValidationSquareDegeneracyException);
                }
            }

            return true;
        }

        // Set a single elements value on this state
        public bool SetElement(int row, int col, sbyte value)
        {
            // Validate the request
            if (value < 1 || value > 9) return false;
            if (row < 1 || row > 9) return false;
            if (col < 1 || col > 9) return false;

            // Validate the actual value
            if (GetConjunctiveElements(row, col).Contains(value))
                return false;

            // Set and return true
            BoardState[row, col] = value;
            return true;
        }

        internal sbyte[] GetRowElements(int row)
        {
            var buffer = new List<sbyte>();

            for (int i = 0; i < 9; i++)
                if (BoardState[row, i] != 0)
                    buffer.Add(BoardState[row, i]);

            return buffer.ToArray();
        }

        internal sbyte[] GetColumnElements(int col)
        {
            var buffer = new List<sbyte>();

            for (int i = 0; i < 9; i++)
                if (BoardState[i, col] != 0)
                    buffer.Add(BoardState[i, col]);

            return buffer.ToArray();
        }

        internal sbyte[] GetSquareElements(int row, int col)
        {
            var buffer = new List<sbyte>();

            var horizOffset = 3 * (col / 3);
            var verticalOffset = 3 * (row / 3);

            for (int i = verticalOffset; i < verticalOffset + 3; i++)
                for (int j = horizOffset; j < horizOffset + 3; j++)
                    if (BoardState[i, j] != 0)
                        buffer.Add(BoardState[i, j]);

            return buffer.ToArray();
        }

        // Get array of distinct elements that share a row, column, or square with the cell specified
        public sbyte[] GetConjunctiveElements(int row, int col)
            => GetRowElements(row)
                .Union(GetColumnElements(col))
                .Union(GetSquareElements(row, col))
                .Distinct().ToArray();

        public override int GetHashCode()
            => BoardState.GetHashCode();

        public bool Equals(SudokuState other)
            => this == null || other == null
            ? false
            : other.GetHashCode().Equals(GetHashCode());
    }
}
