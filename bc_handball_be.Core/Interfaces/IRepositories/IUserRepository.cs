using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        // Read operations
        Task<Login?> GetLoginByUsernameAsync(string username);
        Task<IEnumerable<Person>> GetAllUsersAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<string> GetUserRoleAsync(int personId);

        // Write operations
        Task AddUserWithRoleAsync(Person person, Login login, object roleEntity);
    }
}
