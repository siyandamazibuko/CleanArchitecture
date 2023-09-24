using System;
using CleanArchitecture.Messages.Command.Users;
using FluentValidation;

namespace CleanArchitecture.Application.Validators.Users
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(v => v.UserId).NotEmpty();
            RuleFor(v => v.UserId).NotEqual(Guid.Empty);
        }
    }
}
