using System.Collections.Generic;
using System.Linq;

namespace IAE.Microservice.Application.Common.Operation
{
    public class OperationResult<T>
    {
        protected OperationResult(T data = default, bool succeeded = false, params OperationError[] errors)
        {
            Data = data;
            Succeeded = succeeded;
            if (errors != null)
            {
                _errors.AddRange(errors);
            }
        }

        /// <summary>
        /// Result data.
        /// </summary>
        /// <value>Data of <see cref="T"/></value>
        public T Data { get; }

        /// <summary>
        /// Flag indicating whether if the operation succeeded or not.
        /// </summary>
        /// <value>True if the operation succeeded, otherwise false.</value>
        public bool Succeeded { get; }

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> of <see cref="OperationError"/>s containing an errors
        /// that occurred during the operation.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="OperationError"/>s.</value>
        public IEnumerable<OperationError> Errors => _errors;

        private readonly List<OperationError> _errors = new List<OperationError>();

        /// <summary>
        /// Returns an <see cref="OperationResult{T}"/> indicating a successful operation, with a <see cref="T"/>
        /// </summary>
        /// <param name="data">A data of <see cref="T"/>.</param>
        /// <returns>An <see cref="OperationResult{T}"/> indicating a successful operation, with a <see cref="T"/></returns>
        public static OperationResult<T> Success(T data)
        {
            return new OperationResult<T>(data, true);
        }

        /// <summary>
        /// Creates an <see cref="OperationResult{T}"/> indicating a failed operation, with a list of <paramref name="errors"/> if applicable.
        /// </summary>
        /// <param name="data">A data of <see cref="T"/>.</param>
        /// <param name="errors">An optional array of <see cref="OperationError"/>s which caused the operation to fail.</param>
        /// <returns>An <see cref="OperationResult{T}"/> indicating a failed operation, with a list of <paramref name="errors"/> if applicable.</returns>
        public static OperationResult<T> Failed(T data = default, params OperationError[] errors)
        {
            return new OperationResult<T>(data, false, errors);
        }

        /// <summary>
        /// Creates an <see cref="OperationResult{T}"/> indicating a failed operation.
        /// </summary>
        /// <param name="result">Existed failed operation.</param>
        /// <param name="newData">A data of <see cref="T"/>.</param>
        /// <returns>An <see cref="OperationResult{T}"/> indicating a failed operation.</returns>
        public static OperationResult<T> Failed<TIn>(OperationResult<TIn> result, T newData = default)
        {
            return Failed(newData, result.Errors.ToArray());
        }

        /// <summary>
        /// Creates an <see cref="OperationResult{T}"/> indicating a failed operation.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="data">A data of <see cref="T"/>.</param>
        /// <returns>An <see cref="OperationResult{T}"/> indicating a failed operation.</returns>
        public static OperationResult<T> Failed(string code, string description, T data = default)
        {
            return Failed(data, OperationError.Init(code, description));
        }

        /// <summary>
        /// Converts the value of the current <see cref="OperationResult{T}"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current <see cref="OperationResult{T}"/> object.</returns>
        /// <remarks>
        /// If the operation was successful the ToString() will return "Succeeded" otherwise it returned 
        /// "Failed : " followed by a comma delimited list of error codes from its <see cref="Errors"/> collection, if any.
        /// </remarks>
        public override string ToString()
        {
            return Succeeded
                ? "Succeeded"
                : $"Failed : {string.Join(",", Errors.Select(x => $"{x.Code}->{x.Description}").ToList())}";
        }
    }
}