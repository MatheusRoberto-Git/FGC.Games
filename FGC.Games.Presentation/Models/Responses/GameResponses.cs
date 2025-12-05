namespace FGC.Games.Presentation.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse()
        {
            Message = string.Empty;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> SuccessResult(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResult(string message, T data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public class GameResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public double Rating { get; set; }
        public int TotalSales { get; set; }
    }
}
