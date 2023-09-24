using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Application.Handlers.Users;
using CleanArchitecture.Messages.Queries.Users;
using FluentAssertions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CleanArchitecture.UnitTests.Common
{
    [TestClass]
    public class HandlerRegistrationTests
    {
        [TestMethod]
        public void Check_Handlers_ShouldAlwaysPass()
        {
            var requestTypes = typeof(GetUsersQuery).Assembly.GetTypes()
                .Where(IsRequest)
                .ToList();

            var handlerTypes = typeof(GetUsersQueryHandler).Assembly.GetTypes()
                .Where(IsIRequestHandler)
                .ToList();

            foreach (var requestType in requestTypes) ShouldContainHandlerForRequest(handlerTypes, requestType);
        }

        private static void ShouldContainHandlerForRequest(IEnumerable<Type> handlerTypes, Type requestType)
        {
            handlerTypes.Should().ContainSingle(handlerType => IsHandlerForRequest(handlerType, requestType), $"Handler for type {requestType} expected");
        }

        private static bool IsRequest(Type type)
        {
            return typeof(IBaseRequest).IsAssignableFrom(type);
        }

        private static bool IsIRequestHandler(Type type)
        {
            return type.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
        }

        private static bool IsHandlerForRequest(Type handlerType, Type requestType)
        {
            return handlerType.GetInterfaces().Any(i => i.GenericTypeArguments.Any(ta => ta == requestType));
        }
    }
}
