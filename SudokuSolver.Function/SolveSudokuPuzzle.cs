using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SudokuSolver.Solver.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSolver.Function
{
    public static class SolveSudokuPuzzle
    {
        [FunctionName("SolvePuzzle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ISudokuSolver solver,
            ILogger log)
        {
            // Set up buffer and config variables, with default for config
            var cancelAfterTime = int.Parse(ConfigurationManager.AppSettings["MaximumProcessingTimeSeconds"] ?? "0");
            if (cancelAfterTime < 1 || cancelAfterTime > 240) cancelAfterTime = 120;
            string solution;

            // Log start
            log.LogInformation("API function started");

            // Read body into string
            StreamReader streamReader = new StreamReader(req.Body);
            string boardString = streamReader.ReadToEnd();

            // Validate using regex, 400 if fails
            if (!Regex.IsMatch(boardString, @"^[0-9\.\-\?]{81}$"))
                return new BadRequestObjectResult(ApiResponse.Failure(@"Invalid Sudoku puzzle: must match regex '^[0-9\.\-\?]{81}$'"));

            // Log simple validation
            log.LogInformation("Sudoku puzzle contents and length validated by regular expression.");

            // Validate using rules of sudoku, 400 if fails
            if (!solver.IsValidBoard(boardString))
                return new BadRequestObjectResult(ApiResponse.Failure($"Invalid board state: {{{boardString}}}"));

            // Log validation complete
            log.LogInformation("Sudoku puzzle clues validated. Proceeding to attempt solve.");

            // Try to keep our return civil, so catch everything and use logging
            try
            {
                // Create cancellation token source which cancels after specified time
                using var cts = new CancellationTokenSource(cancelAfterTime * 1000);

                // Wrap process to solve in a task, so we can time-limit
                solution = await Task.Run(() => solver.SolvePuzzle(boardString), cts.Token);
            }
            // Cancelled implies it took too long, 200 with failure
            catch (OperationCanceledException)
            {
                return new OkObjectResult(ApiResponse.Failure("Operation cancelled: timeout."));
            }
            // Dno what happened, 500
            catch (Exception ex)
            {
                log.LogError(ex, "Exception encountered attempting to solve puzzle.");
                return new StatusCodeResult(500);
            }

            // Simple regex validation, should prevent false positives
            if (Regex.IsMatch(solution, @"^[1-9]{81}$"))
            {
                // Log this, but not the actual values
                log.LogInformation("Solution pair calculated and returned.");

                // 200 with solution
                return new OkObjectResult(new ApiResponse(solution, "Success", true));
            }
            else
            {
                // Log failure and return 200 w/ failure
                log.LogInformation("Solution was not found.");
                return new OkObjectResult(ApiResponse.Failure("Failed to identify solution. Unsolvable?"));
            }
        }
    }
}
