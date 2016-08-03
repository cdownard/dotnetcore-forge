using System.Collections.Generic;

namespace Forge.Model
{
    public sealed class ModelDefinition
    {
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public IEnumerable<ModelProperty> Properties { get; set; }
    }
}