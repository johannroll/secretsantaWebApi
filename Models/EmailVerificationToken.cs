using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Models
{
    public class EmailVerificationToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

    }
}
