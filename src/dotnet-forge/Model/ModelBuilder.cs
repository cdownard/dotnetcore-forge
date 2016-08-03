using System;
using System.Linq;

namespace Forge.Model
{
    public class ModelBuilder
    {
        private const string ModelTemplate = @"
          namespace {{namespace}}
          {
            public class {{className}}
            {
              {{properties}}
            }
          }
        ";

        public string Build(ModelDefinition modelDefinition)
        {
            var properties = modelDefinition.Properties
              .Select(p => $"public {p.Type.FullName} {p.Name} {{ get; set; }}{Environment.NewLine}")
              .Aggregate((l, r) => l + r);

            var template = ModelTemplate
              .Replace("{{namespace}}", modelDefinition.Namespace)
              .Replace("{{className}}", modelDefinition.ClassName)
              .Replace("{{properties}}", properties);

            return template;
        }
    }
}
