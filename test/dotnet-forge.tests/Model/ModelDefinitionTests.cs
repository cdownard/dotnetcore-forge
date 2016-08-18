using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Forge.Model.Tests
{
    public class ModelDefinitionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ModelDefinition_Namespace_Validation_Fails(string @namespace)
        {
            var target = new ModelDefinition
            {
                Namespace = @namespace,
                ClassName = "ClassName",
                Properties = new[] { new ModelProperty { Name = "Id", Type = typeof(int) } }
            };

            var validationContext = new ValidationContext(target);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(target, validationContext, validationResults);

            Assert.False(isValid);
            Assert.Equal(1, validationResults.Count);

            var targetResult = validationResults
                .Where(vr => vr.ErrorMessage == "The Namespace field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Namespace")
                .SingleOrDefault();

            Assert.NotNull(targetResult);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ModelDefinition_ClassName_Validation_Fails(string className)
        {
            var target = new ModelDefinition
            {
                ClassName = className,
                Namespace = "SomeNamespace",
                Properties = new[] { new ModelProperty { Name = "Id", Type = typeof(int) } }
            };

            var validationContext = new ValidationContext(target);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(target, validationContext, validationResults);

            Assert.False(isValid);
            Assert.Equal(1, validationResults.Count);

            var targetResult = validationResults
                .Where(vr => vr.ErrorMessage == "The ClassName field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "ClassName")
                .SingleOrDefault();

            Assert.NotNull(targetResult);
        }

        [Fact]
        public void ModelDefinition_Properties_Required_Validation_Fails()
        {
            var target = new ModelDefinition
            {
                ClassName = "SomeClass",
                Namespace = "SomeNamespace",
                Properties = default(IEnumerable<ModelProperty>)
            };

            var validationContext = new ValidationContext(target);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(target, validationContext, validationResults);

            Assert.False(isValid);
            Assert.Equal(1, validationResults.Count());

            var targetResult = validationResults
                .Where(vr => vr.ErrorMessage == "The Properties field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Properties")
                .SingleOrDefault();

            Assert.NotNull(targetResult);
        }

        [Fact]
        public void ModelDefinition_Properties_CollectionElement_Validation_Fails()
        {
            var target = new ModelDefinition
            {
                ClassName = "SomeClass",
                Namespace = "SomeNamespace",
                Properties = new[] { new ModelProperty() }
            };

            var validationContext = new ValidationContext(target);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Equal(1, validationResults.Count);

            var targetResult = validationResults
                .Where(vr => vr.ErrorMessage == "Validation failed for Properties.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Properties")
                .SingleOrDefault();

            Assert.NotNull(targetResult);

            var typedResult = targetResult as CompositeValidationResult;  

            Assert.NotNull(targetResult);
            Assert.Equal(2, typedResult.ValidationResults.Count());

            var nameRequired = typedResult.ValidationResults
                .Where(vr => vr.ErrorMessage == "The Name field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Name")
                .SingleOrDefault();

            Assert.NotNull(nameRequired);

            var typeRequired = typedResult.ValidationResults
                .Where(vr => vr.ErrorMessage == "The Type field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Type")
                .SingleOrDefault();

            Assert.NotNull(typeRequired);
        }
    }
}