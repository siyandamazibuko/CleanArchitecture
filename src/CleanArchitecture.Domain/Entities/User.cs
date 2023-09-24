using System;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities
{
    public partial class User : AuditableEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string Telephone { get; set; }
    }
}
