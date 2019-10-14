using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Solver
{
    internal class SudokuSolutionSpace
    {
        public static readonly sbyte[] _VALID = new sbyte[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public readonly SudokuState _sudokuState;
        public SudokuState SudokuState => _sudokuState;
        public sbyte[,][] solutionSpace = new sbyte[9, 9][];
        public List<(int row, int col, sbyte val)> SolvedCells;
        public int Uncertainty
            => Enumerable.Range(0, 9).Cross(Enumerable.Range(0, 9)).Select(tpl =>
                solutionSpace[tpl.Item1, tpl.Item2].Length).Sum();

        public SudokuSolutionSpace(SudokuState sudokuState)
        {
            if (sudokuState.BoardState == null)
                throw new ArgumentNullException(nameof(sudokuState));

            _sudokuState = sudokuState;

            SolvedCells = CalculateSolutionItemsAndSetSolutionSpace(sudokuState);
        }

        public static List<(int, int, sbyte)> CalculateSolutionItems(SudokuState state)
        {
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

        private List<(int, int, sbyte)> CalculateSolutionItemsAndSetSolutionSpace(SudokuState state)
        {
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
    }
}
