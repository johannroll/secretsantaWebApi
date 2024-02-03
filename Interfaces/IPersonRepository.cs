using SecretSantaApi.Models;

namespace SecretSantaApi.Interfaces
{
    public interface IPersonRepository
    {
        Task<ICollection<Person>> GetAllPeopleAsync();
        Task<ICollection<Person>> GetPeopleByListIdAsync(int listId);
        Task<Person> GetPersonAsync(int personId);
        Task<bool> PersonExistsAsync(int personId);
        Task<bool> CreatePersonAsync(Person person);
        Task<bool> UpdatePersonAsync(Person person);
        Task<bool> DeletePersonAsync(Person person);
        Task<bool> DeleteAllPeopleFromAListAsync(List<Person> people);
        Task<bool> SaveAsync();
    }
}
