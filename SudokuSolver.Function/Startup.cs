using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SudokuSolver.Solver.Implementations;
using SudokuSolver.Solver.Interfaces;

[assembly: FunctionsStartup(typeof(SudokuSolver.Function.Startup))]

namespace SudokuSolver.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<ISudokuSolver, BranchingSudokuSolver>();
        }
    }
}
