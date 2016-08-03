using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using System.Linq;

namespace Forge.Model.Tests
{
    public class ModelTests
    {
        [Fact]
        public void Model_Emits_Valid_Class()
        {
            var model = new Model
            {
                ClassName = "TestClass",
                Properties = new Dictionary<string, string> {
                   { "Index", "int" },
                   { "Name", "string" }
                }
            };

            var output = model.ToString();
            var tree = CSharpSyntaxTree.ParseText(output);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var namespaceDeclaration = root.Members.LastOrDefault() as NamespaceDeclarationSyntax;

            Assert.NotNull(namespaceDeclaration);
            Assert.Equal("YourAppName.Models", namespaceDeclaration.Name.ToString());

            var classDeclaration = namespaceDeclaration.Members.LastOrDefault() as ClassDeclarationSyntax;

            Assert.NotNull(classDeclaration);
            Assert.Equal("TestClass", classDeclaration.Identifier.ToString());

            var corlibAssembly = typeof(object)
                .GetTypeInfo()
                .Assembly;

            var corlib = MetadataReference.CreateFromFile(corlibAssembly.Location);
            var compilation = CSharpCompilation.Create("Model", new [] { tree }, new [] { corlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var properties = classDeclaration
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Select(p => semanticModel.GetDeclaredSymbol(p))
                .Cast<IPropertySymbol>()
                .ToArray();

            var nameProperty = properties
                .Where(p => p.Name == "Name")
                .Where(p => TypeSymbolHelper.TypeSymbolMatchesType(p.Type, typeof(string), semanticModel))
                .SingleOrDefault();

            Assert.NotNull(nameProperty);

            var indexProperty = properties
                .Where(p => p.Name == "Index")
                .Where(p => TypeSymbolHelper.TypeSymbolMatchesType(p.Type, typeof(int), semanticModel))
                .SingleOrDefault();
            
            Assert.NotNull(indexProperty);
        }
    }
}