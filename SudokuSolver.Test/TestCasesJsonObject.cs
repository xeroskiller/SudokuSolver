using Newtonsoft.Json;

namespace SudokuSolver.Test
{
    internal class JsonObject
    {
        public SampleProblem[] Examples { get; set; }

        [JsonProperty("examplesNoSoln")]
        public string[] ExamplesNoSolution { get; set; }
        public string[] InvalidStates { get; set; }
        public string HardestPuzzle { get; set; }
    }

    internal class SampleProblem
    {
        public string InitialState { get; set; }
        public string Solution { get; set; }
    }
}
