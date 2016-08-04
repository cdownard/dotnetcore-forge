using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace Forge.Model
{
    public class ModelBuilder
    {
        public string Build(ModelDefinition definition)
        {
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

            var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(namespaceDeclaration.AddMembers(classDeclaration.AddMembers(properties)));

            var result = new StringBuilder();

            using (var workspace = new AdhocWorkspace())
            {
                var formatted = Formatter.Format(compilationUnit, workspace);
                using(var writer = new StringWriter(result))
                {
                  formatted.WriteTo(writer);
                }
            }

            return result.ToString();
        }
    }
}
