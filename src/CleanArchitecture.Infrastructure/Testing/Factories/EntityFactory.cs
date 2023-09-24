using System;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Testing.Factories
{
    public static class EntityFactory
    {
        public static User CreateUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "TestUser",
                Surname = "TestSurname",
                DateOfBirth = new DateTime(2016,01,01),
                Telephone = "021452369850",
                EmailAddress = "aaaa@gmail.com"
            };

            return user;
        }
    }
}
