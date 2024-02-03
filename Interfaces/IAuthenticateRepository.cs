using SecretSantaApi.Models;

namespace SecretSantaApi.Interfaces
{
    public interface IAuthenticateRepository
    {
        Task<bool> RegisterUserAsync(User user, string password);
        Task<User> LoginUserAsync(string email, string password);
        Task<bool> SaveAsync();
    }
}
