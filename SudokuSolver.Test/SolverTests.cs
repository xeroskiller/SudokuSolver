using SudokuSolver.Solver;
using System;
using Xunit;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SudokuSolver.Test
{
    public class SolverTests
    {
        [Fact]
        public void Solve_ExamplePuzzlesFromJson_SucceedsAndMatches()
        {
            var fileContents = File.ReadAllText("./SudokuExamples.json");
            var sampleSet = JsonConvert.DeserializeObject<JsonObject>(fileContents);

            foreach (var example in sampleSet.Examples)
            {
                var solver = new SudokuSolved(example.InitialBoardState());
                solver.Solve();

                Assert.True(solver.Solved);
                Assert.NotNull(solver.SolvedState);

                Assert.Equal(SampleProblem.StringFromArray2D(solver.SolvedState.BoardState), example.Solution);
            }
        }
    }

    internal class JsonObject
    {
        public SampleProblem[] Examples { get; set; }
    }

    internal class SampleProblem
    {
        public string InitialState { get; set; }
        public string Solution { get; set; }

        public sbyte[,] InitialBoardState()
            => Array2DFromString(InitialState);

        public sbyte[,] SolutionState()
            => Array2DFromString(Solution);
        
        private sbyte[,] Array2DFromString(string value)
        {
            if (InitialState.Length != 81)
                throw new ArgumentException("Too many cells in test case.");

            sbyte[,] buffer = new sbyte[9, 9];

            for (int i = 0; i < 9; i++) for (int j = 0; j < 9; j++)
                    buffer[i, j] = sbyte.Parse(InitialState.ElementAt(9 * i + j).ToString().Replace('.', '0'));

            return buffer;
        }

        public static string StringFromArray2D(sbyte[,] array)
        {
            if (array.GetLength(0) != 9
                || array.GetLength(1) != 9)
                throw new ArgumentException("Must be 9x9");

            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++) for (int j = 0; j < 9; j++)
                    sb.Append(array[i, j]);

            return sb.ToString();
        }
    }
}
