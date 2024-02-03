using Microsoft.EntityFrameworkCore;
using SecretSantaApi.Data;
using SecretSantaApi.Interfaces;
using SecretSantaApi.Models;

namespace SecretSantaApi.Repository
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly SecretSantaApiDbContext _context;
        public EmailVerificationRepository(SecretSantaApiDbContext context)
        {
            _context = context;
        }


        public async Task<EmailVerificationToken> GetTokenAsync(string token)
        {
            return await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<bool> MarkTokenAsUsedAsync(EmailVerificationToken token)
        {
            token.IsUsed = true;
            _context.EmailVerificationTokens.Update(token);
            return await SaveAsync();
        }

        public async Task<bool> SaveTokenAsync(EmailVerificationToken token)
        {
            token.ExpiresAt = DateTime.UtcNow.AddHours(1);
            await _context.EmailVerificationTokens.AddAsync(token);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var save = await _context.SaveChangesAsync();
            return save > 0 ? true : false;
        }
    }
}
