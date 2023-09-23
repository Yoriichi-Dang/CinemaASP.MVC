namespace DUTC.Models.DTO
{
    public class ShowtimeListVm
    {
        public IQueryable<Showtime>? ShowtimeList { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Term { get; set; }
        public string? sortOrder { get; set; }
        public string? search { get; set; }
        public string? type { get; set; }
    }
}
