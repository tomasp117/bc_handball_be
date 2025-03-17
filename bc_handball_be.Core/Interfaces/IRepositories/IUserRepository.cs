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
        Task AddUserAsync(Person user);
        Task<Person?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<Person>> GetAllUsersAsync();
    }
}
