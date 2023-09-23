using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Net.WebSockets;

namespace DUTC.Repository.Implements
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly ApplicationDbContext _ctx;
        public ShowtimeService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public bool Add(Showtime model)
        {
            try
            {//còn trường hợp nếu thêm phim trùng phòng khác giờ mà khoảng thời gian từ giờ nhỏ hơn gần nhất bé hơn Time watch thì không được thêm
                if (!FindExistShowtime(model)&&CheckDate(model))
                {
                    _ctx.Showtimes.Add(model);
                    _ctx.SaveChanges();
                    var idShowtime=_ctx.Showtimes.FirstOrDefault(a=>a.MovieID==model.MovieID&&a.Time==model.Time&&a.RoomID==model.RoomID);
                    if (idShowtime != null)
                    {
                        if (AddRoomShowtime(model)) return true;
                        else return false;

                    }
                    else return false;
                }
                else return false;
            }catch (Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public bool AddRoomShowtime(Showtime model)
        {
            var room = _ctx.Rooms.FirstOrDefault(a => a.RoomID == model.RoomID);
            if (room == null)
            {
                return false;
            }
            int row = (int)room.Row;
            int col = (int)room.Column;
            int k1 = (int)Math.Ceiling((double)row / 4);
            int k2 = (int)Math.Floor((double)(3 * row) / 4);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Seat a = new Seat();
                    a.state = 0;
                    a.ShowtimeRoomID = model.ID;
                    char character = (char)('A' + i);
                    string idSeat = $"{character}{(j + 1):D2}"; // Format mã ghế A01, A02, ..., A20
                    if (i >= 0 && i < k1)
                    {
                        a.TypeSeat = "normal";
                    }
                    else if (i > k2 && i < row)
                    {
                        a.TypeSeat = "normal";
                    }
                    else
                    {
                        a.TypeSeat = "vip";
                    }
                    a.SeatID = idSeat;
                    _ctx.Seats.Add(a);
                    _ctx.SaveChanges();
                }
            }
            return true;
            throw new NotImplementedException();
        }

        public bool CheckDate(Showtime model)
        {
            var datestr = (from movie in _ctx.Movies
                           where movie.Id == model.MovieID
                           select movie.DateReleased
             ).FirstOrDefault();
            DateTime date;
            bool success = DateTime.TryParseExact(datestr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (date <= model.Time.Date && success)
            {
                return true;
            }
            else return false;
        }

        public bool Delete(int id)
        {
            try
            {
                var result=GetById(id);
                _ctx.Showtimes.Remove(result);
                _ctx.SaveChanges();
                if (DeleteRoomShowtime(id)) return true;
                else return false;
            }catch(Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }
        public Movie GetMovieById(int idshowtime)
        {
            var movie = (from m in _ctx.Movies
                         join s in _ctx.Showtimes
                         on m.Id equals s.MovieID
                         where s.ID == idshowtime
                         select m
                ).FirstOrDefault() ;
            return movie;
        }
        public Room GetRoombyId(int idshowtime)
        {
            var room = (from r in _ctx.Rooms
                        join s in _ctx.Showtimes
                        on r.RoomID equals s.RoomID
                        where s.ID == idshowtime
                        select r
                ).FirstOrDefault() ;
            return room;
        }
        public bool DeleteRoomShowtime(int idShowtime)
        {
            var listSeat = _ctx.Seats.Where(a => a.ShowtimeRoomID == idShowtime).ToList();
            if (listSeat.Count > 0)
            {
                foreach (var item in listSeat)
                {
                    _ctx.Seats.Remove(item);
                    _ctx.SaveChanges();
                }
                return true;
            }
            return false;
            throw new NotImplementedException();
        }
        public List<Seat> getSeats(int idShowtime)
        {
            return _ctx.Seats.Where(a => a.ShowtimeRoomID == idShowtime).ToList();
            throw new NotImplementedException();
        }
        public bool FindExistShowtime(Showtime model)
        {
            //check showtime
            var data=_ctx.Showtimes.FirstOrDefault(a=>a.RoomID==model.RoomID&&a.Time.ToString()==model.Time.ToString());
            if (data != null) return true;
            //get list showtime by  in date and roomid
            var list = _ctx.Showtimes.Where(a=>a.Time.Date==model.Time.Date&&a.RoomID==model.RoomID).OrderBy(a=>a.Time).ToList();
            if(list.Count > 0)
            {
                var movie = _ctx.Movies.FirstOrDefault(a => a.Id == model.MovieID);
                if (movie != null)
                {
                    model.Title = movie.Title;
                    model.EndWatch = model.Time.AddMinutes((int)movie.TimeWatch + 20);
                    //set end watch for list movie
                    foreach (var item in list)
                    {
                        var movieItem= _ctx.Movies.FirstOrDefault(a => a.Id == item.MovieID);
                        item.EndWatch = item.Time.AddMinutes((int)movieItem.TimeWatch + 20);
                    }
                    int i = 0;
                    while (i < list.Count && model.Time > list[i].EndWatch)
                    {
                        i++;
                    }
                    if (i == list.Count)
                    {
                        return false;
                    }
                    else
                    {
                        if(i > 0)
                        {
                            var pre = list[i - 1];
                            if (pre.EndWatch >= model.Time) return true;
                        }
                        if (list[i].Time <= model.EndWatch) return true;
                    }
                }
                else return true;
            }
            return false;
        }
        public bool MakeCopyShowtimeMovie(int id)//movieid
        {
            var list=_ctx.Showtimes.Where(a=>a.MovieID==id).OrderBy(a => a.Time).ToList();
            var latest = list[list.Count - 1];
            list = list.Where(a => a.Time.Date == latest.Time.Date).ToList();
            foreach (var item in list)
            {
                Showtime model= new Showtime();
                model.MovieID = item.MovieID;
                model.RoomID = item.RoomID;
                model.Time = item.Time.AddDays(1);
                var result=Add(model);
            }
            return true;
        }
        public Showtime GetById(int id)
        {
            return _ctx.Showtimes.Find(id);
            throw new NotImplementedException();
        }

        public IQueryable<Showtime> GetListByNameMovie(string movieName)
        {
            var data = from movie in _ctx.Movies
                       join show in _ctx.Showtimes on movie.Id equals show.MovieID
                       where movie.Title == movieName
                       select show;
            data = data.OrderBy(a => a.Time);
            return data;
        }

        public ShowtimeListVm List(string term = "",string sortOrder="", bool paging = false, int currentPage = 0, string search = "", string type = "")
        {
            var vm = new ShowtimeListVm();
            vm.Term=term;
            vm.sortOrder=sortOrder;
            vm.search = search;
            var list = _ctx.Showtimes.OrderBy(a => a.Time).ToList();
            foreach (var showtime in list)
            {
                var title = (from s in _ctx.Showtimes
                             join movie in _ctx.Movies
                             on s.MovieID equals movie.Id
                             where s.ID == showtime.ID
                             select movie.Title
                              ).FirstOrDefault();
                if (title != null)
                {
                    showtime.Title = title;
                }
            }
            //type
            type= type.ToLower();
            if (!string.IsNullOrEmpty(type)&&type.ToLower()!="all")
            {
                vm.type= type;
                DateTime currentDate= DateTime.Now;
                if(type.Contains("not"))
                {
                    list = list.Where(a => a.Time > currentDate).ToList();
                }
                else
                {
                    list = list.Where(a => a.Time <= currentDate).ToList();
                }
            }
            if(!string.IsNullOrEmpty(term))
            {
                list=list.Where(a=>a.Title.ToLower().StartsWith(term)||a.RoomID.ToString()==term).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                DateTime date=DateTime.Parse(search);
                string datestr=date.Date.ToString("dd/MM/yyyy");
                list=list.Where(a=>a.Time.Date.ToString("dd/MM/yyyy")==datestr).ToList();
                if (list.Any())
                {
                    var listTime= list.Where(a => a.Time.ToString("dd/MM/yyyy hh\\:mm") == date.ToString("dd/MM/yyyy hh\\:mm")).ToList();
                    if (listTime.Any())
                    {
                        list = listTime;
                    }
                }
            }
            if (!sortOrder.IsNullOrEmpty())
            {
                if (sortOrder.Contains("title"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list=list.OrderByDescending(a=>a.Title).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.Title).ToList();
                    }
                }
                else if (sortOrder.Contains("room"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.RoomID).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.RoomID).ToList();
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.Time).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.Time).ToList();
                    }
                }
            }
            else
            {
                list = list.OrderByDescending(a => a.Time).ToList();
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                vm.PageSize = pageSize;
                vm.CurrentPage = currentPage;
                vm.TotalPages = TotalPages;
            }
            vm.ShowtimeList = list.AsQueryable();
            return vm;
        }

        public bool Update(Showtime model)
        {
            try
            {
                //find movieshowtime exist or not
                var result = (from m in _ctx.Movies
                              where m.Title.ToLower().Equals(model.Title.ToLower())
                              select m.Id
                      ).FirstOrDefault();
                if (result != null)
                {
                    model.MovieID = result;
                    if (!FindExistShowtime(model) && CheckDate(model))
                    {
                            _ctx.Showtimes.Update(model);
                            _ctx.SaveChanges();
                            return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }
        public ShowtimeListVm GetShowtimeOnDate()
        {
            var list=_ctx.Showtimes.Where(a=>a.Time>=DateTime.Now&&a.Time.Date.ToString()==DateTime.Now.Date.ToString()).OrderBy(a=>a.Time).ToList();
            foreach(var item in list)
            {
                var movie=(from s in _ctx.Showtimes join m in _ctx.Movies on s.MovieID equals m.Id where s.ID==item.ID select m).FirstOrDefault();
                if (movie != null)
                {
                    item.Title=movie.Title;
                    item.EndWatch = item.Time.AddMinutes((int)movie.TimeWatch+10);
                }
                item.TimePlayMovie = item.Time.TimeOfDay.ToString("hh\\:mm")+" - "+item.EndWatch.TimeOfDay.ToString("hh\\:mm");
            }
            ShowtimeListVm showtimeListVm = new ShowtimeListVm();
            showtimeListVm.ShowtimeList=list.AsQueryable();
            return showtimeListVm;
        }
    }
}
