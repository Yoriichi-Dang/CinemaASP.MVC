namespace DUTC.Models.DTO
{
    public class GenreListVm
    {
        public IQueryable<Genre>? Genres { get; set; }
        public string? sortOrder { get; set; }
        public string? term { get; set; }
    }
}
