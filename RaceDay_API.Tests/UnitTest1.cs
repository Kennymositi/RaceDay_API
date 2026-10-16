using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RaceDay_API;
using Xunit;
using RaceDay_API.Controllers;

namespace RaceDay_API.Tests
{
    public class RegisterRequestTests
    {
        private static IList<ValidationResult> ValidateModel(
            object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            Validator.TryValidateObject(
                model, context, results, validateAllProperties: true);

            return results;
        }

        [Fact]
        public void ValidRegistration_HasNoValidationErrors()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "alex@example.com",
                phone = "0712345678",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.Empty(errors);
        }

        [Fact]
        public void MissingFirstName_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "",
                last_name = "Smith",
                email = "alex@example.com",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void InvalidEmail_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "not-an-email",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ShortPassword_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "alex@example.com",
                password = "short",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }
    }
}