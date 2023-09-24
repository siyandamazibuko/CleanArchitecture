using System.Collections.Generic;
using CleanArchitecture.Models.Users;

namespace CleanArchitecture.Messages.Responses.Users
{
    public class UsersResponse
    {
        public IList<User> Users { get; set; }
    }
}
