namespace DUTC.Models.DTO
{
    public class ShowtimeMovieVm
    {
        public string Date { get;set; }
        public List<Tuple<int,string>>? Time { get; set; }
    }
}
