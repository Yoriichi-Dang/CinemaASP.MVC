using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class Showtime
    {
        [Key]
        public int ID { get;set; }
        [Required]
        public int MovieID { get;set; }//movieID
        [Required]
        public DateTime Time { get;set; }
        [Required]
        public int RoomID { get; set; }
        [NotMapped]
        public string? Title { get; set; }
        [NotMapped]
        public Room? room { get; set; }
        [NotMapped]
        public Movie? movie { get; set; }
        [NotMapped]
        public List<Seat>? seats { get; set; }
        [NotMapped]
        public List<string>? SeatBuy { get; set; }
        [NotMapped]
        public int Total {  get; set; }
        [NotMapped]
        public DateTime EndWatch { get; set; }
        [NotMapped]
        public string? TimePlayMovie { get;set; }
    }
}
