using CleanArchitecture.Models.Users;
using FluentValidation;

namespace CleanArchitecture.Application.Validators.Users
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(v => v.Name).NotEmpty();
            RuleFor(v => v.Name).MaximumLength(100);
            RuleFor(v => v.Surname).NotEmpty();
            RuleFor(v => v.Surname).MaximumLength(100);
            RuleFor(v => v.DateOfBirth).NotNull();
            RuleFor(v => v.EmailAddress).EmailAddress();
            RuleFor(v => v.EmailAddress).MaximumLength(100);
            RuleFor(v => v.Telephone).MaximumLength(100);
        }
    }
}
