using CleanArchitecture.Messages.Queries.Users;
using FluentValidation;

namespace CleanArchitecture.Application.Validators.Users
{
    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
        }
    }
}
