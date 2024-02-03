
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecretSantaApi.Data;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;

namespace SecretSantaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SecretSantaApiDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserRepository(SecretSantaApiDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return await SaveAsync();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            return await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.UserEmail == email).FirstOrDefaultAsync();
        }

        public async Task<ICollection<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task<bool> SaveAsync()
        {
            var save = await _context.SaveChangesAsync();
            return save > 0 ? true : false;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.UserId = user.UserId;
            existingUser.UserName = user.UserName;
            existingUser.UserEmail = user.UserEmail;

            // _context.Users.Update(user);
            return await SaveAsync();
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            return await SaveAsync();  
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            user.Password = _passwordHasher.HashPassword(user, password);
            await _context.Users.AddAsync(user);

            return await SaveAsync();
        }

        public async Task<User> LoginUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }

        public async Task<bool> UpdatePasswordAsync(User user, string oldPassword, string newPassword)
        {
            
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, oldPassword) != PasswordVerificationResult.Success)
            {
                return false;
            }

            user.Password = _passwordHasher.HashPassword(user, newPassword);
            _context.Users.Update(user);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ResetPasswordAsync(User user, string newPassword)
        {

            user.Password = _passwordHasher.HashPassword(user, newPassword);
            _context.Users.Update(user);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.UserEmail == email);
        }

        public async Task<bool> SaveUserResetToken(int userId, string resetToken, DateTime resetTokenExpiry)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }            

            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpires = resetTokenExpiry;
        
            return await SaveAsync();
        }

        public Task<User> GetUserByResetTokenAsync(string token)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task<bool> UpdateUserPasswordResetTokenStatus(User user)
        {
            user.PasswordResetToken = "";
            user.PasswordResetTokenExpires = null;
            _context.Users.Update(user);
            return await SaveAsync();
        }
    }
}
