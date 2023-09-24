using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Common.Data;
using CleanArchitecture.Domain.Repositories;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;

        public UsersRepository(IUnitOfWork unitOfWork, IRepository<User> userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _userRepository.GetAllAsync();

        }
        public async Task<Guid> CreateUser(User user)
        {
            _userRepository.Create(user);
            await _unitOfWork.CommitAsync();
            return user.Id;
        }

        public async Task DeleteUser(Guid userId)
        {
            _userRepository.Delete(userId);
            await _unitOfWork.CommitAsync();
        }
    }
}
