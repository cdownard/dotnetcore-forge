using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Forge.Model
{
    public sealed class ModelDefinition
    {
        [Required]
        public string Namespace { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public IEnumerable<ModelProperty> Properties { get; set; }

        public ModelDefinition()
        {
            Properties = Enumerable.Empty<ModelProperty>();
        }
    }
}