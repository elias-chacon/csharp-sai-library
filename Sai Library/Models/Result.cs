// Sai_Library/Models/Result.cs

namespace Sai_Library.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Data { get; }
        public string ErrorMessage { get; }
        public IReadOnlyDictionary<string, object> Metadata { get; }

        private Result(bool isSuccess, T data, string errorMessage,
            Dictionary<string, object> metadata)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            Metadata = metadata != null
                ? new Dictionary<string, object>(metadata)
                : new Dictionary<string, object>();
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, string.Empty,
                new Dictionary<string, object>());
        }

        public static Result<T> Success(T data, Dictionary<string, object> metadata)
        {
            return new Result<T>(true, data, string.Empty, metadata);
        }

        public static Result<T> Error(string message)
        {
            return new Result<T>(false, default, message,
                new Dictionary<string, object>());
        }
    }
}