using System.Collections;
using CleanArchitecture.Messages.Responses.Users;
using CleanArchitecture.Models.Users;
using MediatR;

namespace CleanArchitecture.Messages.Queries.Users
{
    public class GetUsersQuery : IRequest<UsersResponse>
    {
    }
}
