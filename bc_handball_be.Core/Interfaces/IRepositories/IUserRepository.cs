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
        Task<Login?> GetLoginByUsernameAsync(string username);
        //Task AddUserAsync(Person user, string username, string password);
        Task AddUserWithRoleAsync(Person user, Login login, object roleEntity);
        Task<IEnumerable<Person>> GetAllUsersAsync();
        Task<string> GetUserRoleAsync(int personId);
    }
}
