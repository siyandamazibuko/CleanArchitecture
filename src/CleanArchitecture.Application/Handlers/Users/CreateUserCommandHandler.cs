using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Messages.Commands.Users;
using MediatR;

namespace CleanArchitecture.Application.Handlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            return new Guid();
        }
    }
}
