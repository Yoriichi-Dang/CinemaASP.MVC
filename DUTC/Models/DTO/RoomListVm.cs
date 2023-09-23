namespace DUTC.Models.DTO
{
    public class RoomListVm
    {
        public IQueryable<Room>? ListRoom { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? sortOrder { get; set; }
        public string? Term { get; set; }
    }
}
