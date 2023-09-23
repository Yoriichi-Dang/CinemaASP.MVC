using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class CustomerReview
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? UserID { get;set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public DateTime?Time { get; set; }
        [NotMapped]
        [Required]
        public string? Name { get; set; }
        [NotMapped]
        [Required]
        public string? ImageProfile { get; set; }
    }
}
