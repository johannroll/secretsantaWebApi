using SecretSantaApi.Models;

namespace SecretSantaApi.Interfaces
{
    public interface IEmailVerificationRepository
    {
        Task<bool> SaveTokenAsync(EmailVerificationToken token);
        Task<EmailVerificationToken> GetTokenAsync(string token);
        Task<bool> MarkTokenAsUsedAsync(EmailVerificationToken token);
        Task<bool> SaveAsync();
    }
}
