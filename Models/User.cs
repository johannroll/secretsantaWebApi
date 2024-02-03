using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Models
{
    public class User
    {
        public User()
        {
            Lists = new HashSet<GiftList>();
        }

        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; }
        [Required]
        public string Password { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? PasswordResetTokenExpires { get; set; }

        public ICollection<GiftList> Lists { get; set; }
    }
}
