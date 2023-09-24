using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Domain.Repositories;
using CleanArchitecture.Messages.Commands.Users;
using MediatR;

namespace CleanArchitecture.Application.Handlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;

        public CreateUserCommandHandler(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var userEntity = _mapper.Map<Domain.Entities.User>(command.User);
            var userId = await _usersRepository.CreateUser(userEntity);
            return userId;
        }
    }
}
