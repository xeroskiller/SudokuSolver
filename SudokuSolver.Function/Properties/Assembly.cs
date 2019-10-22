using System.Runtime.CompilerServices;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(SudokuSolver.Function.Startup))]
[assembly: InternalsVisibleTo("SudokuSolver.Test")]