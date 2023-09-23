using System.ComponentModel.DataAnnotations.Schema;

namespace DUTC.Models.DTO
{
    public class TicketListVm
    {
        public Ticket ticket { get; set; }
        public List<Seat>? seats { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public int MovieId { get;set; }
        public string? ImagePoster { get; set; }
        public DateTime? TimeShowtime { get; set; }
        public int? RoomID { get; set; }
    }
}
