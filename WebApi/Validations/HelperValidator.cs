using FluentValidation;

namespace WebApi.Validations
{
    // Helper validators
    public class AlphanumericValidator : AbstractValidator<string>
    {
        public AlphanumericValidator(string propertyName, bool required = false, int maxLength = 0, int minLength = 0)
        {
            if (required)
            {
                RuleFor(x => x).NotEmpty().WithMessage($"{propertyName} is required.");
            }

            if (minLength > 0)
            {
                RuleFor(x => x).MinimumLength(minLength).WithMessage($"{propertyName} must be at least {minLength} characters long.");
            }

            if (maxLength > 0)
            {
                RuleFor(x => x).MaximumLength(maxLength).WithMessage($"{propertyName} is too long.");
            }

            RuleFor(x => x).Matches("^[a-zA-Z0-9]+$").When(x => !string.IsNullOrEmpty(x)).WithMessage($"{propertyName} must be alphanumeric.");
        }
    }

    public class NumericValidator : AbstractValidator<string>
    {
        public NumericValidator(string propertyName, bool required = false, int maxLength = 0)
        {
            if (required)
            {
                RuleFor(x => x).Matches("^[0-9]+$").When(x => !string.IsNullOrEmpty(x)).WithMessage($"{propertyName} must be numeric.");

                if (maxLength > 0)
                {
                    RuleFor(x => x).MaximumLength(maxLength).WithMessage($"{propertyName} is too long.");
                }
            }
        }
    }

    public class PostalCodeValidator : AbstractValidator<string>
    {
        public PostalCodeValidator(string propertyName, bool required = false)
        {
            if (required)
            {
                RuleFor(x => x).Matches("^[A-Za-z]\\d[A-Za-z] \\d[A-Za-z]\\d$").When(x => !string.IsNullOrEmpty(x))
                .WithMessage($"Please specify a valid {propertyName}.")
                .MaximumLength(7).WithMessage($"{propertyName} is too long.");
            }
        }
    }
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator(string propertyName, bool required = false)
        {
            if (required)
            {
                RuleFor(x => x).NotEmpty().WithMessage($"{propertyName} is required.");
            }

            RuleFor(x => x).EmailAddress().WithMessage($"{propertyName} must be a valid email address.");
        }
    }
}
