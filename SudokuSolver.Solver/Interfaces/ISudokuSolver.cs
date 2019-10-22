namespace SudokuSolver.Solver.Interfaces
{
    public interface ISudokuSolver
    {
        sbyte[,] SolvePuzzle(sbyte[,] boardState);
        string SolvePuzzle(string boardState);
        bool IsValidBoard(string boardString);
    }
}
