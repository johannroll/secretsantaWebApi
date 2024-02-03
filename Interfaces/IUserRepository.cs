
using SecretSantaApi.Models;

namespace SecretSantaApi.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsersAsync();
        Task<User> GetUserAsync(int userId);
        Task<User> GetUserByEmailAsync(string eamil);
        Task<User> GetUserByResetTokenAsync(string token);
        Task<bool> SaveUserResetToken(int userId, string resetToken, DateTime resetTokenExpiry);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
        Task<bool> RegisterUserAsync(User user, string password);
        Task<User> LoginUserAsync(string username, string password);
        Task<bool> UpdatePasswordAsync(User user, string oldPassword, string newPassword);
        Task<bool> ResetPasswordAsync(User user, string newPassword);
        Task<bool> UpdateUserPasswordResetTokenStatus(User user);
        Task<bool> SaveAsync();
    }
}
