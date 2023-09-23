using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    [Table("ReviewMovie")]
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? UserID { get; set; }
        [Required]
        public int? MovieID { get; set; }
        [Required]
        public string? ContentReview { get; set;}
        [Required]
        public int? Rate { get; set; }
        [Required]
        public DateTime? TimeReview { get; set; }
        [NotMapped]
        public string? UserInfo { get;set; }
        [NotMapped]
        public string? ImageMovie { get;set; }
        [NotMapped]
        public string? MovieTitle { get; set; }
        [NotMapped]
        public string? UserImage { get; set; }
        [NotMapped]
        public string? Date { get; set; }
        [NotMapped]
        public string? Time { get; set; }
    }
}
