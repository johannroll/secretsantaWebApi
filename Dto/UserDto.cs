using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Username length can't be more than 50 characters.")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; }
        public string VerificationToken { get; set; }

    }
}
