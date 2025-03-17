using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person?> GetUserByUsernameAsync(string username)
        {
            return await _context.Persons.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(Person user)
        {
            _context.Persons.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Person>> GetAllUsersAsync()
        {
            return await _context.Persons.ToListAsync();
        }

    }
}
