using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface IShowtimeService
    {
        IQueryable<Showtime> GetListByNameMovie(string movieName);
        bool Add(Showtime model);
        bool Update(Showtime model);
        bool Delete(int id);
        Showtime GetById(int id);
        Movie GetMovieById(int id);
        Room GetRoombyId(int roomId);
        bool FindExistShowtime(Showtime model);
        bool CheckDate(Showtime model);
        List<Seat> getSeats(int idShowtime);
        bool AddRoomShowtime(Showtime model);
        bool DeleteRoomShowtime(int idShowtime);
        ShowtimeListVm List(string term = "",string sortOrder="", bool paging = false, int currentPage = 0,string search="",string type="");
        bool MakeCopyShowtimeMovie(int id);
        ShowtimeListVm GetShowtimeOnDate();
    }
}
