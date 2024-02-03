using SecretSantaApi.Dto;

namespace SecretSantaApi.Interfaces
{
    public interface IEmailService
    {
        public Task SendVerificationEmail(VerifyEmailDto request);

        public Task PasswordResetEmail(PasswordResetEmailDto request);

        public Task SendSecretSantasEmail(ICollection<PersonDto> santas);
    }
}
