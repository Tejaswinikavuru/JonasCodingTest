using FluentValidation;
using System;
using WebApi.Validations;
using WebApi.Models;

namespace WebApi
{
    public class EmployeeInfoValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeInfoValidator()
        {
            RuleFor(x => x.EmployeeCode).SetValidator(new AlphanumericValidator("EmployeeCode", required: true));
            RuleFor(x => x.EmployeeName).SetValidator(new AlphanumericValidator("EmployeeName", required: true, maxLength: 50));
            RuleFor(x => x.CompanyName).SetValidator(new AlphanumericValidator("CompanyName", required: true, maxLength: 100));
            RuleFor(x => x.OccupationName).SetValidator(new AlphanumericValidator("OccupationName", required: false, maxLength: 100));
            RuleFor(x => x.EmployeeStatus).SetValidator(new AlphanumericValidator("EmployeeStatus", required: true, maxLength: 20));
            RuleFor(x => x.EmailAddress).SetValidator(new EmailValidator("EmailAddress", required: true));
            RuleFor(x => x.PhoneNumber).SetValidator(new NumericValidator("PhoneNumber", required: false, maxLength: 10));
            RuleFor(x => x.LastModifiedDateTime).LessThanOrEqualTo(DateTime.Now).WithMessage("LastModifiedDateTime must not be in the future.");
        }
    }
   
}
