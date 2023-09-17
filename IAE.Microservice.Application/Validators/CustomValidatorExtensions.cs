using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IAE.Microservice.Application.Validators
{
    public static class CustomValidatorExtensions
    {
        public static IRuleBuilderOptions<T, long> Id<T>(
            this IRuleBuilder<T, long> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(default(long));
        }

        public static IRuleBuilderOptions<T, long?> Id<T>(
            this IRuleBuilder<T, long?> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(default(long));
        }
        
        public static IRuleBuilderOptions<T, string> Phone<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.MaximumLength(15).SetValidator(new PhoneValidator());
        }

        public static IRuleBuilderOptions<T, string> Password<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.MinimumLength(1).SetValidator(new ContainsDigitValidator());
        }
    }
}