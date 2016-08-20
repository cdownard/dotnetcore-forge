using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Forge.Tests
{
    public class Scratch
    {
        public IEnumerable<INamedTypeSymbol> GetTypes(Compilation compilation)
        {
            var results = new List<INamedTypeSymbol>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var nodes = root.DescendantNodes(n => true);

                var st = root.SyntaxTree;
                var sm = compilation.GetSemanticModel(st);

                if (nodes != null)
                {
                    var syntaxNodes = nodes as SyntaxNode[] ?? nodes.ToArray();

                    var namedTypes = syntaxNodes
                         .OfType<IdentifierNameSyntax>()
                         .Select(id => sm.GetSymbolInfo(id).Symbol)
                         .OfType<INamedTypeSymbol>()
                         .ToArray();

                    var expressionTypes = syntaxNodes
                        .OfType<ExpressionSyntax>()
                        .Select(ex => sm.GetTypeInfo(ex).Type)
                        .OfType<INamedTypeSymbol>()
                        .ToArray();

                    results.AddRange(namedTypes);
                    results.AddRange(expressionTypes);
                }
            }

            return results;
        }

        private DocumentInfo GetDocumentInfo(ProjectId projectId, string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var folders = Path
                .GetDirectoryName(path)
                .Split(Path.DirectorySeparatorChar)
                .Where(pathComponent => !string.IsNullOrEmpty(pathComponent))
                .ToArray();

            var sourceText = SourceText.From(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));

            var textAndVersion = TextAndVersion.Create(
              sourceText,
              VersionStamp.Create(),
              filePath: path);

            var textLoader = TextLoader.From(textAndVersion);

            var info = DocumentInfo.Create(
                DocumentId.CreateNewId(projectId),
                name,
                folders: folders,
                sourceCodeKind: SourceCodeKind.Regular,
                loader: textLoader,
                filePath: path,
                isGenerated: false
            );

            return info;
        }

        [Fact]
        public async void Extract_Types_Test()
        {
            const string TargetPath = "/Users/joshua.hampton/Projects/dotnetcore-forge/src/dotnet-forge";
            const string SearchPattern = "*.cs";

            var results = Directory.GetFiles(TargetPath, SearchPattern, SearchOption.AllDirectories);

            using (var workspace = new AdhocWorkspace())
            {
                var projectInfo = ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    VersionStamp.Create(),
                    "ASDF",
                    "ASDF",
                    LanguageNames.CSharp);

                var project = workspace.AddProject(projectInfo);

                var documents = results
                    .Select(path => GetDocumentInfo(project.Id, path))
                    .Select(documentInfo => workspace.AddDocument(documentInfo))
                    .ToArray();

                project = workspace.CurrentSolution.Projects.SingleOrDefault();

                Assert.NotNull(project);

                var compilation = await project.GetCompilationAsync();

                var types = GetTypes(compilation)
                    .Distinct()
                    .ToArray();
            }
        }

        [Fact]
        public void Get_Compiled_Assembly_References()        
        {

        }
    }
}