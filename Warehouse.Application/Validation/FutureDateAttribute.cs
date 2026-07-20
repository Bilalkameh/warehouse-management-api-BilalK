using System.ComponentModel.DataAnnotations;

namespace Warehouse.Application.Validation;

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date && date.Date <= DateTime.UtcNow.Date)
        {
            return new ValidationResult(
                "Expiry date must be in the future.");
        }

        return ValidationResult.Success;
    }
}