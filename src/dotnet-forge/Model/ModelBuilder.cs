using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;

namespace Forge.Model
{
    public class ModelBuilder
    {
        public ModelBuilderResult Build(ModelDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(definition.Namespace));

            var classDeclaration = SyntaxFactory
                .ClassDeclaration(definition.ClassName)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            var properties = definition.Properties
                .Select(p => new { Name = p.Name, Type = SyntaxFactory.ParseTypeName(p.Type.FullName) })
                .Select(p => SyntaxFactory.PropertyDeclaration(p.Type, p.Name))
                .Select(p => p.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword))))
                .Select(p => p.AddAccessorListAccessors(
                  SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                  SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                ))
                .ToArray();

            var combined = namespaceDeclaration.AddMembers(classDeclaration.AddMembers(properties));
            var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(combined);

            var result = new StringBuilder();
            var emitResult = default(EmitResult);

            using (var workspace = new AdhocWorkspace())
            {
                var solutionInfo = SolutionInfo.Create(
                    SolutionId.CreateNewId(),
                    VersionStamp.Create());

                var solution = workspace.AddSolution(solutionInfo);

                var references = definition.Properties
                    .Select(p => p.Type.GetTypeInfo())
                    .Select(ti => ti.Assembly.Location)
                    .Distinct()
                    .Select(al => MetadataReference.CreateFromFile(al))
                    .ToArray();

                var projectInfo = ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    VersionStamp.Create(),
                    "Forge",
                    "Forge",
                    LanguageNames.CSharp,
                    metadataReferences: references);

                var project = workspace.AddProject(projectInfo);

                var document = project.AddDocument($"{definition.ClassName}", compilationUnit);

                var compilation = project.GetCompilationAsync().Result;

                using (var ms = new MemoryStream())
                {
                    emitResult = compilation.Emit(ms);
                }

                var formatted = Formatter.Format(compilationUnit, workspace);
                using (var writer = new StringWriter(result))
                {
                    formatted.WriteTo(writer);
                }
            }

            return new ModelBuilderResult(result.ToString(), emitResult);
        }
    }
}