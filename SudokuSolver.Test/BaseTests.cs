using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SudokuSolver.Test
{
    public class BaseTests
    {
        private static JsonObject testCases { get; set; }
        public static IEnumerable<object[]> ValidStates => testCases?.ExamplesNoSolution.Select(i => new[] { i });
        public static IEnumerable<object[]> InvalidStates => testCases?.InvalidStates?.Select(i => new[] { i });
        public static IEnumerable<object[]> KnownSolutionStates => testCases?.Examples?.Select(ex => new[] { ex.InitialState, ex.Solution });

        static BaseTests()
        {
            var fileContents = File.ReadAllText("./SudokuExamples.json");
            testCases = JsonConvert.DeserializeObject<JsonObject>(fileContents);
        }
    }
}
