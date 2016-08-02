using System;
using System.Collections.Generic;
using Forge.Model;
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

            var properties = classDeclaration
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToArray();

            // var nameProperty = properties
            //     .Where(p => p.Identifier.ToString() == "Name")
            //     .Where(p => p.Type.)
        }
    }
}