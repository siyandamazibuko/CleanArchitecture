using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Messages.Command.Users;
using MediatR;

namespace CleanArchitecture.Application.Handlers.Users
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IMapper _mapper;

        public DeleteUserCommandHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
