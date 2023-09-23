using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class Movie
    {
        [Key]
        public int Id { get;set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int? TimeWatch { get; set; }
        [Required]
        public string? Cast { get; set; }
        [Required]
        public string? Director { get; set; }
        [Required]
        public string? Nation { get; set; }
        [Required]
        public string? ImagePosterMovie { get; set; }
        [Required]
        public string? VideoPosterMovie { get; set;}
        [Required]
        public string? DateReleased { get;set; }
        [Required]
        public string? DateEnd { get; set; }

        [NotMapped]
        public IFormFile? Imagefile { get; set; }
        [NotMapped]
        public IFormFile? Videofile { get; set; }
        [NotMapped]
        [Required]
        public List<int>? Genres { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? GenreList { get; set; }
        [NotMapped]
        public string? GenreNames { get; set; }
        [NotMapped]
        public MultiSelectList? MultiGenreList { get; set; }
        [NotMapped]
        [Required]
        public List<Showtime>? ListShowtimes { get; set; }
        [NotMapped]
        [Required]
        public int Viewer { get; set; }
        [NotMapped]
        [Required]
        public double RateMovie { get; set; }
        [NotMapped]
        [Required]
        public int TotalRate { get; set; }
        [NotMapped]
        [Required]
        public double Reviewer { get; set; }
        [NotMapped]
        [Required]
        public List<ShowtimeMovieVm>? showtimeMovieVm { get; set; }
        [NotMapped]
        [Required]
        public string? ContentReview { get;set; }
    }
}
