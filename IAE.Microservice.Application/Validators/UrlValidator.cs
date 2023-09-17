using FluentValidation.Validators;
using IAE.Microservice.Application.Validators.Helpers;

namespace IAE.Microservice.Application.Validators
{
    public class UrlValidator : PropertyValidator
    {
        public UrlValidator() : base("{PropertyValue} has incorrect url format")
        {
        }

        public UrlValidator(string errorMessage) : base(errorMessage)
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;
            var url = context.PropertyValue as string;
            return UrlValidationHelper.IsValidUrl(url);
        }
    }
}
