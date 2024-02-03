using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class SetPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string UserEmail { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}
