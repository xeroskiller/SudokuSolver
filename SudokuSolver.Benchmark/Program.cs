using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SudokuSolver.Solver.Implementations;
using System.Collections.Generic;

namespace SudokuSolver.Benchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<SudokuSolverBM>();
        }
    }

    [RPlotExporter, RankColumn, MemoryDiagnoser]
    public class SudokuSolverBM
    {
        #region TestPuzzles
        [ParamsSource(nameof(ValuesFor_PuzzleString))]
        public string PuzzleString { get; set; }

        public IEnumerable<string> ValuesFor_PuzzleString => new[]
        {
            "100007090030020008009600500005300900010080002600004000300000010040000007007000300",
            "000000070060010004003400200800003050002900700040080009020060007000100900700008060",
            "100500400009030000070008005001000030800600500090007008004020010200800600000001002",
            "080000001007004020600300700002009000100060008030400000001700600090008005000000040",
            "100400800040030009009006050050300000000001600000070002004010900700800004020004080",
            // "700603000900000200000000000309100000000050802000000400020040000000700030080000000", // This one is really hard
        };
        #endregion

        [Benchmark]
        public void Benchmark()
            => new BranchingSudokuSolver().SolvePuzzle(PuzzleString);
    }
}
