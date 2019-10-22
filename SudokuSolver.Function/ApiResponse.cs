namespace SudokuSolver.Function
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public ApiResponse(T value)
        {
            Result = value;
            Success = true;
            Message = "Success";
        }

        public ApiResponse(T value, string message)
        {
            Result = value;
            Message = message;
            Success = true;
        }

        public ApiResponse(T value, string message, bool success)
        {
            Success = success;
            Result = value;
            Message = message;
        }

        public static ApiResponse<U> Failure<U>(string message = "")
            => new ApiResponse<U>(default, message, false);
    }

    public class ApiResponse : ApiResponse<string> 
    { 
        public ApiResponse() 
            : base(string.Empty) { }

        public ApiResponse(string message)
            : base(string.Empty, message) { }

        public ApiResponse(string message, bool success)
            : base(string.Empty, message, success) { }

        public ApiResponse(string value, string message, bool success)
            : base(value, message, success) { }

        public static ApiResponse Failure(string message = "")
            => new ApiResponse(message, false);
    }
}
