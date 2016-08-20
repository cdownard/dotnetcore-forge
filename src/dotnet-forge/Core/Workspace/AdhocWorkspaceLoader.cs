using System;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Forge
{
    public sealed class AdhocWorkspaceLoader : IWorkspaceLoader<AdhocWorkspace>
    {
        private const string SearchPattern = "*.cs";

        public string Path { get; }

        public AdhocWorkspaceLoader(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException($"{nameof(path)} cannot be null or whitespace", nameof(path));

            Path = path;
        }

        public AdhocWorkspace Load()
        {
            var csharpFiles = default(string[]);

            try
            {
                csharpFiles = Directory.GetFiles(Path, SearchPattern, SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                var message = $"Unable to search {Path} for *.cs files.";
                throw new Exception(message, e);
            }

            throw new NotImplementedException();
        }
    }
}