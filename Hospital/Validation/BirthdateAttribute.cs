using System.ComponentModel.DataAnnotations;

namespace Hospital.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class BirthDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                if (birthDate > DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage = "Birth date can't be later than today");
                }
            }

            return ValidationResult.Success;
        }
    }
}
