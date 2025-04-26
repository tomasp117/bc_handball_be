using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonRepository> _logger;
        public PersonRepository(ApplicationDbContext context, ILogger<PersonRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddPersonAsync(Person person)
        {
            _logger.LogInformation("Adding person: {Person}", person);
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePersonAsync(Person person)
        {
            _logger.LogInformation("Updating person: {Person}", person);
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePersonAsync(Person person)
        {
            _logger.LogInformation("Deleting person: {Person}", person);
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }
        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            _logger.LogInformation("Getting person by ID: {Id}", id);
            return await _context.Persons
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            _logger.LogInformation("Getting all persons");
            return await _context.Persons
                .ToListAsync();
        }
    }

}
