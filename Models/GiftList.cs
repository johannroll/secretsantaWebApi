using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSantaApi.Models
{
    public class GiftList
    {
        [Key]
        public int ListId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters.")]
        public string Title { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        //Navigation property for related people 
        public ICollection<Person> Participants { get; set; } = new List<Person>();
    }
}
