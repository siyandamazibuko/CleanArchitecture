using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Messages.Queries.Users;
using CleanArchitecture.Messages.Responses.Users;
using MediatR;

namespace CleanArchitecture.Application.Handlers.Users
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, UsersResponse>
    {
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<UsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
