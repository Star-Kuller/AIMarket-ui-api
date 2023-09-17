using System.Linq;
using System.Net;

namespace IAE.Microservice.Application.Common.Operation
{
    public class OperationApiResult<T> : OperationResult<T>
    {
        private OperationApiResult(T data = default, bool succeeded = false,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError, params OperationError[] errors)
            : base(data, succeeded, errors)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }

        public static OperationApiResult<T> Success(T data, HttpStatusCode statusCode)
        {
            return new OperationApiResult<T>(data, true, statusCode);
        }

        public static OperationApiResult<T> Failed(T data = default,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError, params OperationError[] errors)
        {
            return new OperationApiResult<T>(data, false, statusCode, errors);
        }

        public static OperationApiResult<T> Failed<TIn>(OperationApiResult<TIn> result, T newData = default)
        {
            return Failed(newData, result.StatusCode, result.Errors.ToArray());
        }

        public static OperationApiResult<T> Failed(string code, string description, T data,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return Failed(data, statusCode, OperationError.Init(code, description));
        }

        public static OperationApiResult<T> Failed(string code, string description,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return Failed(code, description, default, statusCode);
        }

        public override string ToString()
        {
            return Succeeded
                ? "Succeeded"
                : $"Failed ({StatusCode}): {string.Join(",", Errors.Select(x => $"{x.Code}->{x.Description}").ToList())}";
        }
    }
}