using System;
using MediatR;

namespace CleanArchitecture.Messages.Command.Users
{
    public class DeleteUserCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
