using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Forge
{
    public sealed class CompositeValidationResult : ValidationResult
    {
        public IEnumerable<ValidationResult> ValidationResults { get; }

        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames, IEnumerable<ValidationResult> validationResults) : base(errorMessage, memberNames)
        {
            ValidationResults = validationResults ?? Enumerable.Empty<ValidationResult>();
        }
    }
}