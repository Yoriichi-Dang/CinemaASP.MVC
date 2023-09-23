using System.ComponentModel.DataAnnotations;

namespace DUTC.Models.DTO
{
    public class SeatBuy
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TicketID { get;set; }
        [Required]
        public int SeatID { get; set; }
    }
}
