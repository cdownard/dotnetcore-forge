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
    public sealed class ModelBuilder
    {
        private const string ProjectName = "Forge";
        private const string AssemblyName = "Forge";

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

            var modelDeclaration = namespaceDeclaration.AddMembers(classDeclaration.AddMembers(properties));

            var syntaxResult = new StringBuilder();
            var compileResult = default(EmitResult);

            using (var workspace = new AdhocWorkspace())
            {
                var references = definition.Properties
                    .Select(property => property.Type.GetTypeInfo())
                    .Select(typeInfo => typeInfo.Assembly.Location)
                    .Distinct()
                    .Select(assemblyLocation => MetadataReference.CreateFromFile(assemblyLocation))
                    .ToArray();

                var projectInfo = ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    VersionStamp.Create(),
                    ProjectName,
                    AssemblyName,
                    LanguageNames.CSharp,
                    metadataReferences: references);

                var project = workspace.AddProject(projectInfo);

                var document = project.AddDocument($"{definition.ClassName}", modelDeclaration);

                var compilation = project.GetCompilationAsync().Result;

                using (var ms = new MemoryStream())
                {
                    compileResult = compilation.Emit(ms);
                }

                var formattedModelDeclaration = Formatter.Format(modelDeclaration, workspace);
                using (var writer = new StringWriter(syntaxResult))
                {
                    formattedModelDeclaration.WriteTo(writer);
                }
            }

            return new ModelBuilderResult(syntaxResult.ToString(), compileResult);
        }
    }
}