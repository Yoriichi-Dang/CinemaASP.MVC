namespace DUTC.Models.DTO
{
    public class ListShowtimeMovieVm
    {
        public List<ShowtimeMovieVm> List { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Term { get; set; }
        public string? Type { get; set; }
        public string? sortOrder { get; set; }
    }
}
