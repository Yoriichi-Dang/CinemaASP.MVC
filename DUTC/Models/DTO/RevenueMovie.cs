using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class RevenueMovie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MovieID { get; set; }
        [Required]
        public int Viewer { get;set; }
        [Required]
        public int Rate { get; set; }
        [Required]
        public int Reviewer { get; set; }
        [Required]
        public int TotalMoneyRevenue { get; set; }
        [NotMapped]
        [Required]
        public string? Title { get; set; }
        [NotMapped]
        [Required]
        public string? ImagePosterMovie { get; set; }
        [NotMapped]
        [Required]
        public int Percent { get; set; }
    }
}
