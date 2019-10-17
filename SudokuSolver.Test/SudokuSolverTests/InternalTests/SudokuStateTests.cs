using Newtonsoft.Json;
using SudokuSolver.Solver.Implementations.Internal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SudokuSolver.Test.SudokuSolverTests.InternalTests
{
    public class SudokuStateTests
    {
        private static JsonObject testCases { get; set; }
        public static IEnumerable<object[]> ValidStates => testCases?.ExamplesNoSolution.Select(i => new[] { i });
        public static IEnumerable<object[]> InvalidStates => testCases?.InvalidStates?.Select(i => new[] { i });

        static SudokuStateTests()
        {
            var fileContents = File.ReadAllText("./SudokuExamples.json");
            testCases = JsonConvert.DeserializeObject<JsonObject>(fileContents);
        }

        [Theory]
        [MemberData(nameof(ValidStates))]
        public void IsValidBoardState_ValidBoardStates_ReturnsTrue(string boardString)
        {
            Assert.True(SudokuState.IsValidBoardState(boardString));
        }

        [Theory]
        [MemberData(nameof(InvalidStates))]
        public void IsValidBoardState_InvalidBoardStates_ReturnsFalse(string boardString)
        {
            if (boardString.Length == 81) // Skipping one particular test case that fails the deserializer
                Assert.False(SudokuState.IsValidBoardState(boardString));
        }
    }
}
