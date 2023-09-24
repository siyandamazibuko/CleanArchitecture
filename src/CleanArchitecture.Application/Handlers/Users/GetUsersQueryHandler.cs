using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Domain.Repositories;
using CleanArchitecture.Messages.Queries.Users;
using CleanArchitecture.Messages.Responses.Users;
using CleanArchitecture.Models.Users;
using MediatR;

namespace CleanArchitecture.Application.Handlers.Users
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, UsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;

        public GetUsersQueryHandler(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        public async Task<UsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var userEntities = await _usersRepository.GetUsers() ?? new List<Domain.Entities.User>();
            var users = _mapper.Map<IList<User>>(userEntities);

            return new UsersResponse
            {
                Users = users
            };
        }
    }
}
