using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Repositories
{
    public interface IUsersRepository
    {
        Task<List<User>> GetUsers();
        Task<Guid> CreateUser(User user);
        Task DeleteUser(Guid userId);

    }
}
