using System;
using System.Threading.Tasks;
using CleanArchitecture.Common.Extensions;
using CleanArchitecture.Messages.Command.Users;
using CleanArchitecture.Messages.Commands.Users;
using CleanArchitecture.Messages.Queries.Users;
using CleanArchitecture.Messages.Responses.Users;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [ResponseCache(NoStore = true)]
    public class UsersController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<UsersResponse>> GetUsers()
        {
            var response = await Mediator.Send(new GetUsersQuery());
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var response = await Mediator.Send( new DeleteUserCommand {UserId = userId.ToGuid()});
            return Ok(response);
        }
    }
}
