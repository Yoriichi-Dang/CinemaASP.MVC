namespace DUTC.Models.DTO
{
    public class TicketList
    {
        public IQueryable<TicketListVm>? List { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Term { get; set; }
        public string? sortOrder { get; set; }
    }
}
