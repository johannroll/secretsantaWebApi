using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class PersonDto
    {
        public int PersonId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public bool IsBuyer { get; set; }
        public string giverGiftee { get; set; } = string.Empty;

        //Foreign key for list
        public int ListId { get; set; }
    }
}
