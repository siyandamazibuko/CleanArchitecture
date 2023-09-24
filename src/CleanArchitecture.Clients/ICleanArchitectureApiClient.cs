using Refit;
using CleanArchitecture.Messages.Queries.Users;
using CleanArchitecture.Messages.Responses.Users;
using System.Threading.Tasks;
using CleanArchitecture.Messages.Commands.Users;
using System;

namespace CleanArchitecture.Clients
{
    public interface IRtcbPaymentsApiClient
    {
        #region Users

        [Get("/v1/users")]
        Task<ApiResponse<UsersResponse>> GetUsers();

        [Post("/v1/users")]
        Task<ApiResponse<Guid>> CreateUser(CreateUserCommand command);

        [Delete("/v1/users")]
        Task DeleteUser(string userId);

        #endregion
    }
}
