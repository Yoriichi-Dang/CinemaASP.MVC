using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class Seat
    {   
        [Key]
        public int Id { get; set; }
        [Required]
        public int? ShowtimeRoomID { get;set; }
        [Required]
        public string? SeatID { get; set; }
        [Required]
        public string? TypeSeat { get;set; }
        [Required]
        public int? state { get; set; }
    }
}
