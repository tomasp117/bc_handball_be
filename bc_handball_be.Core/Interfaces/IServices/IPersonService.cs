using bc_handball_be.Core.Entities.Actors.super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Interfaces.IServices
{
    public interface IPersonService
    {
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int id);
        Task<Person?> GetPersonByIdAsync(int id);
        Task<IEnumerable<Person>> GetAllPersonsAsync();
    }
}
