using CleanArchitecture.Messages.Commands.Users;
using FluentValidation;

namespace CleanArchitecture.Application.Validators.Users
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(a => a.User).SetValidator(new UserValidator());
        }
    }
}
