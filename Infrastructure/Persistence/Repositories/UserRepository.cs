using Domain.Entities;
using Domain.Repositories;
using JwtApi.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SchoolApiContext _context;

        public UserRepository(SchoolApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return (IEnumerable<User>)await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
