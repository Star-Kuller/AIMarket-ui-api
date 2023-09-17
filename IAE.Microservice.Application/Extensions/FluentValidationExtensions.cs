using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using IAE.Microservice.Application.Common.Operation;
using Microsoft.AspNetCore.Identity;

namespace IAE.Microservice.Application.Extensions
{
    public static class FluentValidationExtensions
    {
        public static List<ValidationFailure> GetValidationFailures(this IdentityResult result)
        {
            return result.Errors
                .Select(err => new ValidationFailure("", err.Description))
                .ToList();
        }

        public static List<ValidationFailure> GetValidationFailures<T>(this OperationResult<T> result)
        {
            return result.Errors
                .Select(err => new ValidationFailure(err.Code, err.Description))
                .ToList();
        }
    }
}