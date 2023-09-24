using System;
using CleanArchitecture.Models.Users;
using MediatR;

namespace CleanArchitecture.Messages.Commands.Users
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public User User { get; set; }
    }
}
