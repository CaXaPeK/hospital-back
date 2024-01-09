using System.ComponentModel.DataAnnotations;

namespace Hospital.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ModifiedDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage = "Modification time can't be later than now");
                }
            }

            return ValidationResult.Success;
        }
    }
}
