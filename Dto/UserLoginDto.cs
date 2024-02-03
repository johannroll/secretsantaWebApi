using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
