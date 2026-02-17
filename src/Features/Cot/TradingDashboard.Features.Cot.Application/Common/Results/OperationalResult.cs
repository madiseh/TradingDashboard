namespace TradingDashboard.Application.Common.Results;

public class OperationalResult<T>
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public string? ErrorCode { get; init; }
    public List<string> Errors { get; init; } = [];
    public T? Data { get; init; }

    public static OperationalResult<T> Success(T data, string? message = null)
    {
        return new OperationalResult<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            ErrorCode = null,
            Errors = []
        };
    }

    public static OperationalResult<T> Fail(string message, string? errorCode = null, params string[] errors)
    {
        return new OperationalResult<T>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors?.ToList() ?? [],
            Data = default
        };
    }
}
