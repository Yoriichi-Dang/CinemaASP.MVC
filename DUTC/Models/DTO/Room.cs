using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? RoomID { get; set; }
        [Required]
        public int? Row { get;set; }
        [Required]
        public int? Column { get; set; }
        [NotMapped] 
        [Required]
        public List<Seat> ListSeat { get; set; }
        [NotMapped]
        [Required]
        public int? ShowtimeID {  get; set; }
    }
}
