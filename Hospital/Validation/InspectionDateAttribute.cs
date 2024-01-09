using System.ComponentModel.DataAnnotations;

namespace Hospital.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InspectionDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage = "Inspection date can't be later than today");
                }
            }

            return ValidationResult.Success;
        }
    }
}
