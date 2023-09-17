using FluentValidation.Validators;
using System.Linq;

namespace IAE.Microservice.Application.Validators
{
    public class ContainsDigitValidator : PropertyValidator
    {
        public ContainsDigitValidator() : base("{PropertyName} should contain at least one digit")
        {

        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;
            var value = context.PropertyValue as string;
            return value.Any(char.IsDigit);
        }
    }
}
