using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class PasswordResetDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }

    }
}
