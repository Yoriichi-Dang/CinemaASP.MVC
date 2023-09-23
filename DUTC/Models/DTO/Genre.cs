using System.ComponentModel.DataAnnotations;

namespace DUTC.Models.DTO
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? GenreName { get; set; }
    }
}
