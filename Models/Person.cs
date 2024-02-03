using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSantaApi.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email Adress")]
        public string Email { get; set; }
        public bool IsBuyer { get; set; }
        public string giverGiftee { get; set; } = string.Empty;

        [ForeignKey("GiftList")]
        public int ListId { get; set; }

        //Navigation property for list
        public GiftList List { get; set; }
    }
}
