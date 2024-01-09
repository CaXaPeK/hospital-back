using System.ComponentModel.DataAnnotations;

namespace Hospital.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NextVisitDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date <= DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage = $"Next visit date and time must be later than now {DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss")}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
