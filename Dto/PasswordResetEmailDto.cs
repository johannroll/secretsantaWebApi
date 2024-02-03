namespace SecretSantaApi.Dto
{
    public class PasswordResetEmailDto
    {
        public string To { get; set; } = string.Empty;
        public string ResetLink { get; set; } = string.Empty;

    }

}
