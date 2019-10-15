using System;
using Xunit;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static SudokuSolver.Solver.Solver;

namespace SudokuSolver.Test
{
    public class SolverTests
    {
        private JsonObject testCases { get; set; }

        public SolverTests()
        {
            var fileContents = File.ReadAllText("./SudokuExamples.json");
            testCases = JsonConvert.DeserializeObject<JsonObject>(fileContents);
        }

        [Fact]
        public void Solve_ExamplePuzzlesFromJsonWithSolution_SucceedsAndMatches()
        {
            foreach (var (initial, solution) in testCases.Examples.Select(ex => (ex.InitialState, ex.Solution)))
            {
                var board = DeserializeBoard(initial);
                var solvedBoard = SolvePuzzle(board);

                Assert.NotNull(solvedBoard);
                Assert.True(!(from sbyte m in solvedBoard where m == 0 select 1).Any());

                Assert.Equal(SerializeSudoku(solvedBoard), solution);
            }
        }

        [Fact]
        public void Solve_ExamplePuzzlesFromJsonWithoutSolution_Succeeds()
        {
            foreach (var puzzleString in testCases.ExamplesNoSolution)
            {
                var board = DeserializeBoard(puzzleString);
                var solvedBoard = SolvePuzzle(board);

                Assert.NotNull(solvedBoard);
                Assert.True(!(from sbyte m in solvedBoard where m == 0 select 1).Any());
            }
        }
    }

    internal class JsonObject
    {
        public SampleProblem[] Examples { get; set; }

        [JsonProperty("examplesNoSoln")]
        public string[] ExamplesNoSolution { get; set; }
    }

    internal class SampleProblem
    {
        public string InitialState { get; set; }
        public string Solution { get; set; }
    }
}
