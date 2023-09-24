
using System;

namespace CleanArchitecture.Models.Users
{
    public class User
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string Telephone { get; set; }
    }
}
