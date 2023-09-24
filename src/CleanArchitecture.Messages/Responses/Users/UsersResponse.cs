using System.Collections.Generic;
using CleanArchitecture.Models.Users;

namespace CleanArchitecture.Messages.Responses.Users
{
    public class UsersResponse
    {
        public IList<UserInformation> Users { get; set; }
    }
}
