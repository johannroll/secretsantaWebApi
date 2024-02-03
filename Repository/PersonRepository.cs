using Microsoft.EntityFrameworkCore;
using SecretSantaApi.Data;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;

namespace SecretSantaApi.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly SecretSantaApiDbContext _context;

        public PersonRepository(SecretSantaApiDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreatePersonAsync(Person person)
        {
            await _context.People.AddAsync(person);
            return await SaveAsync();
        }

        public async Task<bool> DeletePersonAsync(Person person)
        {
            _context.People.Remove(person);
            return await SaveAsync();
        }

        public async Task<Person> GetPersonAsync(int personId)
        {
            return await _context.People.Where(p => p.PersonId == personId).FirstOrDefaultAsync();
        }

        public async Task<bool> PersonExistsAsync(int personId)
        {
            return await _context.People.AnyAsync(p => p.PersonId == personId);
        }

        public async Task<bool> UpdatePersonAsync(Person person)
        {
            var existingPerson = await _context.People.FindAsync(person.PersonId);
            if (existingPerson == null)
            {
                return false;
            }

            existingPerson.PersonId = person.PersonId;
            existingPerson.Name = person.Name;
            existingPerson.Email = person.Email;

            // _context.People.Update(existingPerson);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var save = await _context.SaveChangesAsync();
            return save > 0 ? true : false;
        }

        public async Task<ICollection<Person>> GetPeopleByListIdAsync(int listId)
        {
            return await _context.People.Where(p => p.ListId == listId).ToListAsync();
        }

        public async Task<bool> DeleteAllPeopleFromAListAsync(List<Person> people)
        {
            _context.RemoveRange(people);
            return await SaveAsync();
        }

        public async Task<ICollection<Person>> GetAllPeopleAsync()
        {
            return await _context.People.ToListAsync();
        }
    }
}
