using System.ComponentModel.DataAnnotations;

namespace Gymr.Common;

internal class NotEmptyAttribute : ValidationAttribute
{
    public int Min { get; set; } = int.MinValue;

    public int Max { get; set; } = int.MaxValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IEnumerable<object> enumerable)
        {
            return enumerable.Any() ? ValidationResult.Success : new ValidationResult("Field must not be empty", new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}