using System.ComponentModel.DataAnnotations;

namespace SecretSantaApi.Dto
{
    public class GiftListDto
    {
        public int ListId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters.")]
        public string Title { get; set; }

        //Foreign key for User
        public int UserId { get; set; }
    }
}
