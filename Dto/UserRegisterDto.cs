using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string UserEmail { get; set; }
    }
}
