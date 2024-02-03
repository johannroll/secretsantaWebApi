using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class UpdatePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

}

