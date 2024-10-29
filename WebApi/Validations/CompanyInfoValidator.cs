using FluentValidation;
using System.Linq;
using WebApi.Models;
using WebApi.Validations;

namespace WebApi
{
    public class CompanyInfoValidator : AbstractValidator<CompanyDto>
    {
        public CompanyInfoValidator()
        {
            // BaseInfo validations
            RuleFor(x => x.CompanyCode).SetValidator(new AlphanumericValidator("CompanyCode", required: true));
            RuleFor(x => x.SiteId).SetValidator(new AlphanumericValidator("SiteId", required: false));
            RuleFor(x => x.CompanyName).SetValidator(new AlphanumericValidator("CompanyName", required: false, maxLength: 25));
            RuleFor(x => x.AddressLine1).SetValidator(new AlphanumericValidator("AddressLine1", required: false, maxLength: 100));
            RuleFor(x => x.AddressLine2).SetValidator(new AlphanumericValidator("AddressLine2", required: false, maxLength: 100));
            RuleFor(x => x.AddressLine3).SetValidator(new AlphanumericValidator("AddressLine3", required: false, maxLength: 100));
            RuleFor(x => x.PhoneNumber).SetValidator(new NumericValidator("PhoneNumber", required: false, maxLength: 10));
            RuleFor(x => x.FaxNumber).SetValidator(new AlphanumericValidator("FaxNumber", required: false, maxLength: 20));
            RuleFor(x => x.PostalZipCode).SetValidator(new PostalCodeValidator("PostalZipCode"));
            RuleFor(x => x.EquipmentCompanyCode).NotEmpty().MaximumLength(50).WithMessage("EquipmentCompanyCode is too long.");

            // List validation for subledgers
            RuleForEach(x => x.ArSubledgers).SetValidator(new ArSubledgerInfoValidator());
        }
    }

    public class ArSubledgerInfoValidator : AbstractValidator<ArSubledgerDto>
    {
        public ArSubledgerInfoValidator()
        {
            RuleFor(x => x.CompanyCode).SetValidator(new AlphanumericValidator("CompanyCode", required: true, minLength: 3, maxLength: 10));
            RuleFor(x => x.SiteId).SetValidator(new AlphanumericValidator("SiteId", required: true));
            RuleFor(x => x.ArSubledgerCode).SetValidator(new AlphanumericValidator("ArSubledgerCode", required: false));
            RuleFor(x => x.Description).SetValidator(new AlphanumericValidator("Description", required: false));
            RuleFor(x => x.PostalZipCode).SetValidator(new PostalCodeValidator("PostalZipCode"));
            RuleFor(x => x.Active).NotEmpty().Must(BeValidStatus).WithMessage("Active status must be valid.");
        }

        private bool BeValidStatus(string status)
        {
            var validStatuses = new[] { "Active", "Inactive", "Excellent", "Good", "Fair", "Poor", "Condemned" };
            return validStatuses.Contains(status);
        }
    }
}
