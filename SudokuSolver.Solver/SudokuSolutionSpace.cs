using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class SudokuSolutionSpace
    {
        // List of valid values for Sudoku cells
        private static readonly sbyte[] _VALID = new sbyte[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        // Field to store initial state for this solution space
        private readonly SudokuState _sudokuState;
        // Private field for solution space
        private sbyte[,][] solutionSpace = new sbyte[9, 9][];

        // Property to expose initial state
        public SudokuState State => _sudokuState;
        // Property to allow enumeration of solved cells, and their solutions
        public List<(int row, int col, sbyte val)> SolvedCells { get; private set; }
        // Simple Uncertainty metric, calculated by summing the number of possibilities
        //      for each cell, beyond the first.
        public int Uncertainty
            => Enumerable.Range(0, 9).Cross(Enumerable.Range(0, 9)).Select(tpl =>
                solutionSpace[tpl.Item1, tpl.Item2].Length).Sum() - 81;

        // Ctor
        public SudokuSolutionSpace(SudokuState sudokuState)
        {
            // Throw on null board state
            if (sudokuState?.BoardState == null)
                throw new ArgumentNullException(nameof(sudokuState));

            // Initialize solution space
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    solutionSpace[row, col] = Array.Empty<sbyte>();

            // Store the initial state
            _sudokuState = sudokuState;

            // Calculate the solvable cells, as well as solution space
            SolvedCells = CalculateSolutionItemsAndSetSolutionSpace(sudokuState);
        }

        // Identifies all conclusively solvable cells
        public static List<(int, int, sbyte)> CalculateSolutionItems(SudokuState state)
        {
            if (state?.BoardState == null) throw new ArgumentNullException(nameof(state));

            var solvedCells = new List<(int r, int c, sbyte v)>();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (state.BoardState[row, col] != 0) continue;

                    var conjunctiveElements = state.GetConjunctiveElements(row, col);
                    var possibleSolutions = _VALID.Except(conjunctiveElements).ToArray();

                    if (possibleSolutions.Length == 1)
                        solvedCells.Add((row, col, possibleSolutions.Single()));

                    if (possibleSolutions.Length == 0)
                        throw new Exception();
                }
            }

            return solvedCells;
        }

        public SudokuState SolveAll()
        {
            var bufferState = State.BoardState.Clone() as sbyte[,];

            foreach (var (row, col, val) in SolvedCells)
                bufferState[row, col] = val;

            return new SudokuState(bufferState);
        }

        private List<(int, int, sbyte)> CalculateSolutionItemsAndSetSolutionSpace(SudokuState state)
        {
            if (state?.BoardState == null) throw new ArgumentNullException(nameof(state));

            var solvedCells = new List<(int r, int c, sbyte v)>();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (state.BoardState[row, col] != 0) continue;

                    var conjunctiveElements = state.GetConjunctiveElements(row, col);
                    var possibleSolutions = _VALID.Except(conjunctiveElements).ToArray();

                    if (possibleSolutions.Length == 1)
                        solvedCells.Add((row, col, possibleSolutions.Single()));

                    if (possibleSolutions.Length == 0)
                        throw new Exception();

                    solutionSpace[row, col] = possibleSolutions;
                }
            }

            return solvedCells;
        }

        public sbyte[] this[int row, int col] => solutionSpace[row, col];

        public (int row, int col, sbyte[] vals) GetKeystone()
        {
            // Buffer for keystone
            (int row, int col, sbyte val) uncCoords = (-1, -1, sbyte.MaxValue);

            // Loop over all cells, storing as keystone if Uncertainty is less
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (solutionSpace[i, j].Length < uncCoords.val)
                        uncCoords = (i, j, (sbyte)solutionSpace[i, j].Length);

            // Return the appropriate values
            return (uncCoords.row, uncCoords.col, solutionSpace[uncCoords.row, uncCoords.col]);
        }
    }
}
