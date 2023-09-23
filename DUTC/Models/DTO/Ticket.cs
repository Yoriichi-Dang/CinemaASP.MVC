using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? ShowtimeID { get; set; }
        [Required]
        public string?UserID { get; set; }
        [Required]
        public DateTime? DateTime { get; set; }
        [Required]
        public int? Total {  get; set; }
        [NotMapped]
        public List<string>SeatBuys { get; set; }
        [NotMapped]
        public string? UserInfo{ get; set; }
        [NotMapped]
        public string? Title { get;set; }
        [NotMapped]
        public int? CountTicket { get; set; }
        [NotMapped]
        public string? TimePay { get; set; }
    }
}
