using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SudokuSolver.Function;
using SudokuSolver.Solver.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SudokuSolver.Test.FunctionTests
{
    public class SolvePuzzleFunctionTests : BaseTests
    {
        private readonly static ILogger logger = NullLogger.Instance;
        private readonly static string FakeSolution = @"123456789123456789123456789123456789123456789123456789123456789123456789123456789";

        [Theory]
        [MemberData(nameof(KnownSolutionStates))]
        public async Task SolvePuzzle_SuccessfulSolve_200WithSolution(string puzzleString, string solutionString)
        {
            var serviceMock = new Mock<ISudokuSolver>(MockBehavior.Strict);
            serviceMock.Setup(s => s.IsValidBoard(It.IsAny<string>())).Returns(true);
            serviceMock.Setup(s => s.SolvePuzzle(puzzleString)).Returns(solutionString);

            var result = await SolveSudokuPuzzle.Run(CreateMockRequest(puzzleString).Object, serviceMock.Object, logger);

            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);

            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult);
            Assert.NotNull(okObjResult.Value);

            var apiResp = okObjResult.Value as ApiResponse;
            Assert.NotNull(apiResp);
            Assert.Equal(apiResp.Result, solutionString);
            Assert.True(apiResp.Success);
        }

        [Theory]
        [MemberData(nameof(ValidStates))]
        public async Task SolvePuzzle_SuccessfulSolve_200WithMockSolution(string puzzleString)
        {
            var serviceMock = new Mock<ISudokuSolver>(MockBehavior.Strict);
            serviceMock.Setup(s => s.IsValidBoard(It.IsAny<string>())).Returns(true);
            serviceMock.Setup(s => s.SolvePuzzle(puzzleString)).Returns(FakeSolution);

            var result = await SolveSudokuPuzzle.Run(CreateMockRequest(puzzleString).Object, serviceMock.Object, logger);

            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);

            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult);
            Assert.NotNull(okObjResult.Value);

            var apiResp = okObjResult.Value as ApiResponse;
            Assert.NotNull(apiResp);
            Assert.Equal(apiResp.Result, FakeSolution);
            Assert.True(apiResp.Success);
        }

        private static Mock<HttpRequest> CreateMockRequest(string body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.Write(body);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
}
