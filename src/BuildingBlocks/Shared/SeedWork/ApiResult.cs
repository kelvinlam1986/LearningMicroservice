namespace Shared.SeedWork
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; }

        public ApiResult() { }
        public ApiResult(bool isSuccess, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public ApiResult(bool isSuccess, T data, string message)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }
    }
}
