using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Forge
{
    public sealed class CompositeValidationResult : ValidationResult
    {
        public IEnumerable<ValidationResult> ValidationResults { get; }

        public CompositeValidationResult(string errorMessage, IEnumerable<ValidationResult> validationResults) : base(errorMessage)
        {
            ValidationResults = validationResults ?? Enumerable.Empty<ValidationResult>();
        }
    }
}