namespace DUTC.Models.DTO
{
    public class RevenueListVm
    {
        public List<RevenueMovie>List { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? sortOrder { get; set; }
        public string? Term { get; set; }
    }
}
