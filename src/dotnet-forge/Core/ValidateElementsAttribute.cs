using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Forge
{
    public sealed class ValidateElementsAttribute : ValidationAttribute
    {
        private IEnumerable<ValidationResult> ValidateElement(object target, ValidationContext context)
        {
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(target, context, validationResults, true);

            return validationResults.AsReadOnly();
        }

        protected override ValidationResult IsValid(object target, ValidationContext context)
        {
            if (target != null && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(target.GetType()))
            {
                var results = ((IEnumerable)target)
                    .Cast<object>()
                    .SelectMany(item => ValidateElement(item, new ValidationContext(item) { MemberName = context.MemberName, DisplayName = context.DisplayName }))
                    .Where(r => r != ValidationResult.Success)
                    .ToArray();

                if (results.Length != 0)
                {
                    var memberNames = new [] { context.MemberName };

                    return new CompositeValidationResult($"Validation failed for {context.DisplayName}.", memberNames, results);
                }
            }

            return ValidationResult.Success;
        }
    }
}