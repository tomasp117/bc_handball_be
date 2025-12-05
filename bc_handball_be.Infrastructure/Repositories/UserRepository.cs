using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
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

        // ==================== READ OPERATIONS ====================

        public async Task<Login?> GetLoginByUsernameAsync(string username)
        {
            return await _context.Logins
                .Include(l => l.Person)
                .FirstOrDefaultAsync(l => l.Username == username);
        }

        public async Task<IEnumerable<Person>> GetAllUsersAsync()
        {
            return await _context.Persons
                .Include(p => p.Login)
                .ToListAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Logins.AnyAsync(l => l.Username == username);
        }

        public async Task<string> GetUserRoleAsync(int personId)
        {
            // Check role tables in priority order
            if (await _context.Admins.AnyAsync(a => a.PersonId == personId)) return "Admin";
            if (await _context.Coaches.AnyAsync(c => c.PersonId == personId)) return "Coach";
            if (await _context.Recorders.AnyAsync(r => r.PersonId == personId)) return "Recorder";
            if (await _context.Referees.AnyAsync(r => r.PersonId == personId)) return "Referee";
            if (await _context.Players.AnyAsync(p => p.PersonId == personId)) return "Player";
            if (await _context.ClubAdmins.AnyAsync(ca => ca.PersonId == personId)) return "ClubAdmin";

            return "User";
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task AddUserWithRoleAsync(Person person, Login login, object roleEntity)
        {
            // Add all entities in a single transaction for atomicity
            _context.Persons.Add(person);
            _context.Logins.Add(login);

            // Add the appropriate role entity based on type
            switch (roleEntity)
            {
                case Admin admin:
                    _context.Admins.Add(admin);
                    break;
                case Coach coach:
                    _context.Coaches.Add(coach);
                    break;
                case Referee referee:
                    _context.Referees.Add(referee);
                    break;
                case Recorder recorder:
                    _context.Recorders.Add(recorder);
                    break;
                case Player player:
                    _context.Players.Add(player);
                    break;
                case ClubAdmin clubAdmin:
                    _context.ClubAdmins.Add(clubAdmin);
                    break;
                default:
                    throw new ArgumentException("Unknown role type", nameof(roleEntity));
            }

            await _context.SaveChangesAsync();
        }
    }
}
