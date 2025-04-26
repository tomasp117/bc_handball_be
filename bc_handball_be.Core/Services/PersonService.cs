using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Services
{
    public class PersonService : IPersonService
    {

        private readonly IPersonRepository _personRepository;
        private readonly ILogger<PersonService> _logger;


        public PersonService(IPersonRepository personRepository, ILogger<PersonService> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }
        public async Task AddPersonAsync(Person newPerson)
        {
            await _personRepository.AddPersonAsync(newPerson);
        }
        public async Task DeletePersonAsync(int id)
        {
            var person = await _personRepository.GetPersonByIdAsync(id);
            if (person == null) throw new Exception("Osoba nenalezena");
            await _personRepository.DeletePersonAsync(person);
        }
        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await _personRepository.GetAllPersonsAsync();
        }
        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _personRepository.GetPersonByIdAsync(id);
        }

        public Task<Person?> GetPersonByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonAsync(Person person)
        {
            throw new NotImplementedException();
        }
    }
}
