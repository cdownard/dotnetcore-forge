using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forge
{
    public sealed class ValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object target, ValidationContext context)
        {
            if (target != null)
            {
                var results = new List<ValidationResult>();
                Validator.TryValidateObject(target, context, results);

                if (results.Count != 0)
                {
                    var memberNames = new[] { context.MemberName };
                    return new CompositeValidationResult($"Validation failed for {context.DisplayName}", memberNames, results.AsReadOnly());
                }
            }

            return ValidationResult.Success;
        }
    }
}