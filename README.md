# SudokuSolver

Simple Sudoku solver, written in C#, and exposed as a function HTTP endpoint.

## Infrastructure

This web application is written in Angular 8, with an Azure Functions API, and no backend.  
The underlying libraries are written in C#, with minimal external dependency.  
When complete, this application should run as a static website in Azure Storage, with  
a consumption-plan based API/backend.   

This creates a stack which can host the website at the egress rate of Storage, for less  
than ~$10/mo. Here, we are relying on functions automated scaling to manage the large CPU  
requirement of this process. Additionally, AppInsights is used as the primary troubleshooting  
and perf-tracking service.  

#### TODO
This repo also defines an ARM (Azure Resource Manager) template, which can be used to provision  
services in Azure, with the appropriate settings and nomenclature.

## Projects

Currently, projects include:  

* ### SudokuSolver.Solver

This is a C# class library, based on .NET Core 3.0, which takes a sudoku puzzle, as either  
a string, or a 2D sbyte-array, and calculates the solution, if possible. For performance  
reasons, a minumum of 17 clues (solved cells in the initial configuration) is required for  
a puzzle to be solved. This is also the minimum number of clues required to imply a unique  
solution to a Sudoku puzzle (McGuire, Tugemann, Civario) <sup>[[1]](https://arxiv.org/abs/1201.0749)[[2]](http://www.math.ie/checker.html)</sup>  

* ### SudokuSolverApp

This is an Angular 8 application, written using VS Code, allowing a user to manually enter  
a Sudoku puzzle, and retrieve a solution. This app uses prebuilt Angular themes, and minimal  
external dependencies, to facilitate learning. When complete, this application is hosted in  
Azure Storage, as a static website. This is likely the cheapest possible way to host an Angular  
app in Azure.

* ### SudokuSolver.Function

This is an Azure Functions project, which serves as an API for the Angular application. This  
function currently defines a single endpoint.

`GET /SolveSudoku/`

This endpoint requires the Sudoku puzzle, serialized as a single string, be passed in as the  
body of the HTTP request. The response will be of the shape:

```json
{
    "Success": "Boolean value indicating whether this request was successfully fulfilled",
    "Message": "String value describing the result",
    "Result": "String value containing similarly serialized and corresponding Sudoku solution, if possible"
}
```

* ### SudokuSolver.Test

This is an Xunit unit-test project for SudokuSolver.Solver and SudokuSolver.Function. This  
collection of unit tests includes a number of test puzzzles of varying complexity to test  
efficacy, as well as invalid puzzles, unsolvable puzzles, known solutions, and faculties to  
test all methods and endpoints with these. This project also tests the API endpoint, using  
the same puzzles, asserting varying underlying results.