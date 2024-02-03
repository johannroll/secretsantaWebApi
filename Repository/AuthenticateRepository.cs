using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecretSantaApi.Data;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;

namespace SecretSantaApi.Repository
{
    public class AuthenticateRepository : IAuthenticateRepository
    {
        private readonly SecretSantaApiDbContext _context;

        public IPasswordHasher<User> _passwordHasher;

        public AuthenticateRepository(SecretSantaApiDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> SaveAsync()
        {
            var save = await _context.SaveChangesAsync();
            return save > 0 ? true : false;
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            user.Password = _passwordHasher.HashPassword(user, password);
            user.UserName = user.UserEmail;
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
    }
}
