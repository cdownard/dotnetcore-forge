using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Forge.Model.Tests
{
    public class ModelPropertyTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ModelProperty_Name_Validation_Fails(string name)
        {
            var target = new ModelProperty { Name = name, Type = typeof(int) };

            var validationContext = new ValidationContext(target);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(target, validationContext, validationResults);

            Assert.False(isValid);
            Assert.Equal(1, validationResults.Count);

            var targetResult = validationResults
                .Where(vr => vr.ErrorMessage == "The Name field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Name")
                .SingleOrDefault();

            Assert.NotNull(targetResult);
        }

        [Theory]
        [InlineData(null)]
        public void ModelProperty_Type_Validation_Fails(Type type)
        {
            var target = new ModelProperty { Name = "SomeName", Type = type };

            var validationContext = new ValidationContext(target);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(target, validationContext, validationResults);

            Assert.False(isValid);
            Assert.Equal(1, validationResults.Count);

            var targetResult = validationResults
                .Where(vr => vr.ErrorMessage == "The Type field is required.")
                .Where(vr => vr.MemberNames.SingleOrDefault() == "Type")
                .SingleOrDefault();

            Assert.NotNull(targetResult);
        }
    }
}