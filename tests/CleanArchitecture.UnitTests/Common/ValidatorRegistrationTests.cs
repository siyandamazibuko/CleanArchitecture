using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Application.Validators.Users;
using CleanArchitecture.Messages.Queries.Users;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CleanArchitecture.UnitTests.Common
{
    [TestClass]
    public class ValidatorRegistrationTests
    {
        [TestMethod]
        public void Check_Validators_ShouldAlwaysPass()
        {
            var requestTypes = typeof(GetUsersQuery).Assembly.GetTypes()
                .Where(IsRequest)
                .ToList();

            var validatorTypes = typeof(GetUsersQueryValidator).Assembly.GetTypes()
                .Where(IsValidator)
                .ToList();

            foreach (var requestType in requestTypes) ShouldContainValidatorForRequest(validatorTypes, requestType);
        }

        private static void ShouldContainValidatorForRequest(IEnumerable<Type> validatorTypes, Type requestType)
        {
            validatorTypes.Should().ContainSingle(validatorType => IsValidatorForRequest(validatorType, requestType), $"Validator for type {requestType} expected");
        }

        private static bool IsRequest(Type type)
        {
            return typeof(IBaseRequest).IsAssignableFrom(type);
        }

        private static bool IsValidator(Type type)
        {
            return typeof(IValidator).IsAssignableFrom(type);
        }

        private static bool IsValidatorForRequest(Type handlerType, Type requestType)
        {
            return handlerType.GetInterfaces().Any(i => i.GenericTypeArguments.Any(ta => ta == requestType));
        }
    }
}
