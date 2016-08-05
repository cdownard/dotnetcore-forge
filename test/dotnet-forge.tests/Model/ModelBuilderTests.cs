using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using System.Linq;

namespace Forge.Model.Tests
{
    public class ModelBuilderTests
    {
        [Fact]
        public void ModelBuilder_Emits_Valid_Class()
        {
            var definition = new ModelDefinition
            {
                ClassName = "TestClass",
                Namespace = "YourAppName.Models",
                Properties = new[] 
                {
                    new ModelProperty { Name = "Index", Type = typeof(int) },
                    new ModelProperty { Name = "Name", Type = typeof(string) }
                }
            };

            var modelBuilder = new ModelBuilder();

            var buildResult = modelBuilder.Build(definition);
            var tree = CSharpSyntaxTree.ParseText(buildResult.Output);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var namespaceDeclaration = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .SingleOrDefault();

            Assert.NotNull(namespaceDeclaration);
            Assert.Equal("YourAppName.Models", namespaceDeclaration.Name.ToString());

            var classDeclaration = namespaceDeclaration
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .LastOrDefault();

            Assert.NotNull(classDeclaration);
            Assert.Equal("TestClass", classDeclaration.Identifier.ToString());

            var corlibAssembly = typeof(object)
                .GetTypeInfo()
                .Assembly;

            var corlib = MetadataReference.CreateFromFile(corlibAssembly.Location);
            var compilation = CSharpCompilation.Create("Model", new[] { tree }, new[] { corlib });
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

        [Fact]     
        public void ModelBuilder_Fails_On_Duplicate_Property_Names()
        {
            var definition = new ModelDefinition
            {
                ClassName = "TestClass",
                Namespace = "YourAppName.Models",
                Properties = new[] 
                {
                    new ModelProperty { Name = "Index", Type = typeof(int) },
                    new ModelProperty { Name = "Index", Type = typeof(string) }
                }
            };

            var modelBuilder = new ModelBuilder();

            var buildResult = modelBuilder.Build(definition);

            Assert.False(buildResult.CompileResult.Success);
        }
    }
}