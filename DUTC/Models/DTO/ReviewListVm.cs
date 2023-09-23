namespace DUTC.Models.DTO
{
    public class ReviewListVm
    {
        public IQueryable<Review>? List { get; set;}
        public List<Tuple<int,int>>?ListRate { get; set;}
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Title { get; set; }
        public string? Rate { get; set; }
        public string? sortOrder { get; set; }
        public List<string>?TitleMovie { get; set; }
    }
}
