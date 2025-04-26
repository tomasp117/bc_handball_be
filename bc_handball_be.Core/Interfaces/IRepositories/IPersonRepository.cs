using bc_handball_be.Core.Entities.Actors.super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IRepositories
{
    public interface IPersonRepository
    {
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(Person person);
        Task<Person?> GetPersonByIdAsync(int id);
        Task<IEnumerable<Person>> GetAllPersonsAsync();
    }
}
