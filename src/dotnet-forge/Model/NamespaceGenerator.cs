using System;

namespace Forge.Model
{
    public class NamespaceGenerator
    {
        public string Generate(string projectPath)
        {
            if (string.IsNullOrWhiteSpace(projectPath)) throw new ArgumentException($"{nameof(projectPath)} cannot be null or whitespace", nameof(projectPath));

            throw new NotImplementedException();
        }
    }
}