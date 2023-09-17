using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace IAE.Microservice.Application.Validators
{
    public class PhoneValidator : PropertyValidator
    {
        private static readonly Regex PhoneRegex =
            new Regex(@"^\+(?:[0-9] ?){6,14}[0-9]$",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        public PhoneValidator() : base("{PropertyName} has incorrect phone number format")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;
            var phone = context.PropertyValue as string;
            return PhoneRegex.IsMatch(phone);
        }
    }
}
