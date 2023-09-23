using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DUTC.Repository.Implements
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly UserManager<User> _userManager;
        public TicketService(ApplicationDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }
        public async Task<bool> Add(Ticket model)
        {
            if(model == null)
            {
                return false;
            }
            //add table ticket
            _ctx.Tickets.Add(model);
            _ctx.SaveChanges();
            var user = await _userManager.FindByIdAsync(model.UserID);
            user.Payment += model.Total;
            await _userManager.UpdateAsync(user);
            _ctx.SaveChangesAsync();
            // add view revenue movie
            var revenueMovie = (from r in _ctx.Revenues
                                join m in _ctx.Movies on r.MovieID equals m.Id
                                join s in _ctx.Showtimes on m.Id equals s.MovieID
                                where s.ID==model.ShowtimeID
                                select r
                ).FirstOrDefault();
            if( revenueMovie != null )
            {
                revenueMovie.Viewer += model.SeatBuys.Count();
                revenueMovie.TotalMoneyRevenue +=(int)model.Total;
                _ctx.Revenues.Update(revenueMovie);
                _ctx.SaveChanges();
            }
            // add seat buy
            foreach(var seat in model.SeatBuys)
            {
               var seatID = ( from s in _ctx.Seats
                               where s.SeatID.Equals(seat) && s.ShowtimeRoomID==model.ShowtimeID
                               select s.Id
                    ).FirstOrDefault();
                SeatBuy seats = new SeatBuy();
                seats.TicketID = model.Id;
                seats.SeatID = seatID;
                _ctx.SeatsBuy.Add(seats);
                //update state
                var data = _ctx.Seats.Find(seatID);
                data.state = 1;
                _ctx.Seats.Update(data);
                _ctx.SaveChanges();
            }
            return true;
            throw new NotImplementedException();
        }
        public async Task<List<Ticket>> getTicketList()
        {
            List<Ticket>list=_ctx.Tickets.OrderByDescending(a=>a.DateTime).ToList();
            foreach(var item in list)
            {
                var user = await _userManager.FindByIdAsync(item.UserID);
                if (user != null)
                {
                    item.UserInfo = user.Name;
                }
                var movie=(from t in _ctx.Tickets
                           join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                           join m in _ctx.Movies on s.MovieID equals m.Id
                           where item.Id==t.Id
                           select m).FirstOrDefault();
                if (movie != null)
                {
                    item.Title=movie.Title;
                }
                DateTime datetime=(DateTime)item.DateTime;
                item.TimePay =datetime.ToString("dd/MM/yyyy HH:mm");
            }
            return list;
        }
        public async Task<TicketList> getListHistoryByID(string idUser="", string term = "",string sortOrder="", bool paging = false, int currentPage = 0)
        {
            List<Ticket>list = new List<Ticket>();
            if (!string.IsNullOrEmpty(idUser))
            {
                list = _ctx.Tickets.Where(a => a.UserID.Equals(idUser)).OrderByDescending(a => a.DateTime).ToList();
            }
            else
            {
                list = _ctx.Tickets.OrderByDescending(a => a.DateTime).ToList();
            }
            foreach (var item in list)
            {
                var user = await _userManager.FindByIdAsync(item.UserID);
                if (user != null)
                {
                    item.UserInfo = user.Phone;
                }
            }
            if (!string.IsNullOrEmpty(term))
            {
                list=list.Where(a=>a.Id.ToString()==term||a.UserInfo.ToLower().StartsWith(term)).ToList();
            }
            List<TicketListVm> result=new List<TicketListVm>();
            var datavm= new TicketList();
            datavm.Term = term;
            foreach (var item in list)
            {
                TicketListVm vm = new TicketListVm();
                vm.ticket = item;
                vm.Note = "Buy Ticket";
                var data = (from s in _ctx.SeatsBuy
                            join s1 in _ctx.Seats
                            on s.SeatID equals s1.Id
                            where s.TicketID== item.Id
                            select s1
                    ).ToList();
                vm.seats = data;
                var movie = (from t in _ctx.Tickets
                             join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                             join m in _ctx.Movies on s.MovieID equals m.Id
                             where t.Id== item.Id
                             select m
                    ).FirstOrDefault();
                if(movie!=null)
                {
                    vm.Title = movie.Title;
                    vm.MovieId = movie.Id;
                }
                var time = (from t in _ctx.Tickets
                            join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                            where t.Id== item.Id
                            select s.Time
                    ).FirstOrDefault();
                vm.TimeShowtime= time;
                //get room id
                var room = (from t in _ctx.Tickets
                            join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                            where t.Id == item.Id
                            select s.RoomID
                          ).FirstOrDefault();
                if(room!=null)
                {
                    vm.RoomID = room;
                }
                result.Add(vm);
            }
            datavm.sortOrder = sortOrder;
            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.Contains("total"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        result=result.OrderByDescending(a=>a.ticket.Total).ToList();
                    }
                    else
                    {
                        result = result.OrderBy(a => a.ticket.Total).ToList();
                    }
                }
                else if (sortOrder.Contains("timepay"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        result = result.OrderByDescending(a => a.ticket.DateTime).ToList();
                    }
                    else
                    {
                        result = result.OrderBy(a => a.ticket.DateTime).ToList();
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        result = result.OrderByDescending(a => a.Title).ToList();
                    }
                    else
                    {
                        result = result.OrderBy(a => a.Title).ToList();
                    }
                }
            }
            else
            {
                result=result.OrderByDescending(a=>a.ticket.DateTime).ToList();
                datavm.sortOrder = "timepay_desc";
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = result.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                result = result.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                datavm.PageSize = pageSize;
                datavm.CurrentPage = currentPage;
                datavm.TotalPages = TotalPages;
            }
            datavm.List=result.AsQueryable();
            return datavm;
        }
        public TicketListVm getTicketByID(int id)
        {
            TicketListVm result = new TicketListVm();
            var data = _ctx.Tickets.FirstOrDefault(a => a.Id == id);
            result.ticket = data;
            result.Note = "Buy Ticket";
            //get movie
            var movie = (from t in _ctx.Tickets
                         join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                         join m in _ctx.Movies on s.MovieID equals m.Id
                         where t.Id == id
                         select m
                  ).FirstOrDefault();
            result.MovieId = movie.Id;
            result.Title = movie.Title;
            result.ImagePoster = movie.ImagePosterMovie;
            var time = (from t in _ctx.Tickets
                        join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                        where t.Id == id
                        select s.Time
                   ).FirstOrDefault();
            result.TimeShowtime = time;
            //get seat
            var seatList = (from s in _ctx.SeatsBuy
                        join s1 in _ctx.Seats
                        on s.SeatID equals s1.Id
                        where s.TicketID == id
                        select s1
                  ).ToList();
            var room = (from t in _ctx.Tickets
                        join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                        join m in _ctx.Rooms on s.RoomID equals m.RoomID
                        where t.Id == id
                        select m.RoomID
                     ).FirstOrDefault();
            result.RoomID = room;
            result.seats = seatList;
            return result;
        }
    }
}
