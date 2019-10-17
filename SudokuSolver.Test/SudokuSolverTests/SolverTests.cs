using Newtonsoft.Json;
using SudokuSolver.Solver;
using SudokuSolver.Solver.Implementations;
using SudokuSolver.Solver.Implementations.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SudokuSolver.Test
{
    public class SolverTests
    {
        private static JsonObject testCases { get; set; }
        public static IEnumerable<object[]> ValidStates => testCases?.ExamplesNoSolution.Select(i => new[] { i });
        public static IEnumerable<object[]> InvalidStates => testCases?.InvalidStates?.Select(i => new[] { i });
        public static IEnumerable<object[]> KnownSolutionStates => testCases?.Examples?.Select(ex => new[] { ex.InitialState, ex.Solution });

        static SolverTests()
        {
            var fileContents = File.ReadAllText("./SudokuExamples.json");
            testCases = JsonConvert.DeserializeObject<JsonObject>(fileContents);
        }

        [Theory]
        [MemberData(nameof(KnownSolutionStates))]
        public void Solve_ExamplePuzzlesFromJsonWithSolution_SucceedsAndMatches(string initialState, string solutionString)
        {
            var board = initialState.DeserializeBoard();
            var solver = new BranchingSudokuSolver();
            var solvedBoard = solver.SolvePuzzle(board);

            Assert.NotNull(solvedBoard);
            Assert.True(!(from sbyte m in solvedBoard where m == 0 select 1).Any());

            Assert.Equal(solvedBoard.SerializeSudoku(), solutionString);
        }

        [Theory] 
        [MemberData(nameof(ValidStates))]
        public void Solve_ExamplePuzzlesFromJsonWithoutSolution_Succeeds(string puzzleString)
        {
            // Setup
            var board = puzzleString.DeserializeBoard();

            // Act
            var solver = new BranchingSudokuSolver();
            var solvedBoard = solver.SolvePuzzle(board);

            // Non null
            Assert.NotNull(solvedBoard);

            // No zeros in the whole board
            Assert.True(!(from sbyte m in solvedBoard where m == 0 select 1).Any());

            // Matches initial state, except where initial state was unknown
            Assert.Empty(from m in Enumerable.Range(0, 9)
                         from n in Enumerable.Range(0, 9)
                         where board[m, n] != solvedBoard[m, n] && board[m, n] != 0
                         select 1);

            // This throws if it fails any of the rules of sudoku anywhere
            Assert.NotNull(new SudokuState(solvedBoard));
        }

        // Yeah, we're not solving this without a better alg
        //[Theory, InlineData("000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
        //private void Solve_ReallyHardPuzzle_Eventually(string puzzleString)
        //{
        //    var board = puzzleString.DeserializeBoard();

        //    var solver = new BranchingSudokuSolver();
        //    var solvedBoard = solver.SolvePuzzle(board);

        //    // Non null
        //    Assert.NotNull(solvedBoard);

        //    // No zeros in the whole board
        //    Assert.True(!(from sbyte m in solvedBoard where m == 0 select 1).Any());

        //    // This throws if it fails any of the rules of sudoku anywhere
        //    Assert.NotNull(new SudokuState(solvedBoard));
        //}


        [Theory]
        [MemberData(nameof(InvalidStates))]
        public void Solve_InvalidPuzzleState_Throws(string puzzleString)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var board = puzzleString.DeserializeBoard();

                var solver = new BranchingSudokuSolver();

                solver.SolvePuzzle(board);
            });
        }

        [Fact]
        public void Solve_InvalidBoardValues_Throws()
        {
            var board = new sbyte[9, 9]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 0 },
            };

            var solver = new BranchingSudokuSolver();

            Assert.Throws<ArgumentException>(() => solver.SolvePuzzle(board));
        }
    }
}
