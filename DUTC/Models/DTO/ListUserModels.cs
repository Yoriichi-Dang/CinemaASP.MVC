using DUTC.Data;

namespace DUTC.Models.DTO
{
    public class ListUserModels
    {
        public IQueryable<User>?users { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? sortOrder{get; set; }
        public string? Term { get; set; }
        public string? Type { get; set; }
    }
}
