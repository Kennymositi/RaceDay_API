using System;
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

        [Fact]
        public void MissingLastName_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "",
                email = "alex@example.com",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void MissingPassword_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "alex@example.com",
                password = "",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void MissingRole_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "alex@example.com",
                password = "Password123",
                role = ""
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void FirstNameTooLong_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = new string('A', 51),
                last_name = "Smith",
                email = "alex@example.com",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void LastNameTooLong_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = new string('S', 51),
                email = "alex@example.com",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void EmailTooLong_FailsValidation()
        {
            var request = new RegisterRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = new string('a', 245) + "@example.com",
                password = "Password123",
                role = "PARTICIPANT"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ValidLogin_HasNoValidationErrors()
        {
            var request = new LoginRequest
            {
                email = "alex@example.com",
                password = "Password123"
            };

            var errors = ValidateModel(request);

            Assert.Empty(errors);
        }

        [Fact]
        public void MissingLoginEmail_FailsValidation()
        {
            var request = new LoginRequest
            {
                email = "",
                password = "Password123"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void InvalidLoginEmail_FailsValidation()
        {
            var request = new LoginRequest
            {
                email = "not-an-email",
                password = "Password123"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ValidProfileUpdate_HasNoValidationErrors()
        {
            var request = new UpdateProfileRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "alex@example.com",
                phone = "0712345678"
            };

            var errors = ValidateModel(request);

            Assert.Empty(errors);
        }

        [Fact]
        public void MissingProfileFirstName_FailsValidation()
        {
            var request = new UpdateProfileRequest
            {
                first_name = "",
                last_name = "Smith",
                email = "alex@example.com",
                phone = "0712345678"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void InvalidProfileEmail_FailsValidation()
        {
            var request = new UpdateProfileRequest
            {
                first_name = "Alex",
                last_name = "Smith",
                email = "not-an-email",
                phone = "0712345678"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ValidEventRequest_HasNoValidationErrors()
        {
            var request = new EventRequest
            {
                name = "Cape Town Road Race",
                description = "A road running event",
                event_date = new DateTime(2027, 5, 10),
                location = "Cape Town",
                distance = 10.0m,
                event_type_id = 1
            };

            var errors = ValidateModel(request);

            Assert.Empty(errors);
        }

        [Fact]
        public void MissingEventName_FailsValidation()
        {
            var request = new EventRequest
            {
                name = "",
                description = "A road running event",
                event_date = new DateTime(2027, 5, 10),
                location = "Cape Town",
                distance = 10.0m,
                event_type_id = 1
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ZeroEventDistance_FailsValidation()
        {
            var request = new EventRequest
            {
                name = "Cape Town Road Race",
                description = "A road running event",
                event_date = new DateTime(2027, 5, 10),
                location = "Cape Town",
                distance = 0m,
                event_type_id = 1
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void EventNameTooLong_FailsValidation()
        {
            var request = new EventRequest
            {
                name = new string('A', 151),
                event_date = new DateTime(2027, 5, 10),
                location = "Cape Town",
                distance = 10.0m,
                event_type_id = 1
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void MissingEventLocation_FailsValidation()
        {
            var request = new EventRequest
            {
                name = "Cape Town Road Race",
                event_date = new DateTime(2027, 5, 10),
                location = "",
                distance = 10.0m,
                event_type_id = 1
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void InvalidEventTypeId_FailsValidation()
        {
            var request = new EventRequest
            {
                name = "Cape Town Road Race",
                event_date = new DateTime(2027, 5, 10),
                location = "Cape Town",
                distance = 10.0m,
                event_type_id = 0
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ValidCategoryRequest_HasNoValidationErrors()
        {
            var request = new CategoryRequest
            {
                event_id = 1,
                category_name = "Open Men",
                category_type = "AGE",
                category_value = "18-39",
                description = "Open men's age category"
            };

            var errors = ValidateModel(request);

            Assert.Empty(errors);
        }

        [Fact]
        public void InvalidCategoryType_FailsValidation()
        {
            var request = new CategoryRequest
            {
                event_id = 1,
                category_name = "Open Men",
                category_type = "INVALID",
                category_value = "18-39",
                description = "Open men's age category"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void MissingCategoryName_FailsValidation()
        {
            var request = new CategoryRequest
            {
                event_id = 1,
                category_name = "",
                category_type = "AGE",
                category_value = "18-39",
                description = "Open men's age category"
            };

            var errors = ValidateModel(request);

            Assert.NotEmpty(errors);
        }
    }
}