using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Linq;

namespace DUTC.Repository.Implements
{
    public class RevenueMovieService : IRevenueMovieService
    {
        private readonly ApplicationDbContext _ctx;
        public RevenueMovieService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public Tuple<int, double> GetMonthEarning(string currentMonth = "", string lastMonth = "")
        {
            string type = "month";
            RevenueListVm item1 = GetList(type, currentMonth, false, 1);
            RevenueListVm item2 = GetList(type, lastMonth, false, 1);
            if (item1 != null&&item2!=null)
            {
                var totalMonthPaymentCurrentMonth = 0;
                var totalMonthPaymentLastMonth = 0;
                foreach (var item in item1.List)
                {
                    totalMonthPaymentCurrentMonth += item.TotalMoneyRevenue;
                }
                foreach(var item in item2.List)
                {
                    totalMonthPaymentLastMonth += item.TotalMoneyRevenue;
                }
                double denta = 0;
                if (totalMonthPaymentLastMonth != 0)
                {
                    denta = totalMonthPaymentCurrentMonth * 100 / totalMonthPaymentLastMonth;
                }
                else denta = 100;
                double roundedDenta = Math.Round(denta, 1);
                roundedDenta -= 100;
                Tuple<int, double> result = new Tuple<int, double>(totalMonthPaymentCurrentMonth, roundedDenta);
                return result;
            }
            return null;
        }
        public RevenueListVm GetYearEarning(string year = "")
        {
            RevenueListVm vm = new RevenueListVm();
            var list = _ctx.Tickets.ToList();
            List<Ticket> ticketList = new List<Ticket>();
            if (!string.IsNullOrEmpty(year))
            {
                foreach(var item in list)
                {
                    DateTime datetime = (DateTime)item.DateTime;
                    if(datetime.Year.ToString() == year)
                    {
                        ticketList.Add(item);
                    }
                }
            }
            else
            {
                DateTime date = DateTime.Now;
                foreach (var item in list)
                {
                    DateTime datetime = (DateTime)item.DateTime;
                    if (datetime.Year == date.Year)
                    {
                        ticketList.Add(item);
                    }
                }
            }
            foreach (var item in list)
            {
                var movie = (from t in _ctx.Tickets
                             join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                             join m in _ctx.Movies on s.MovieID equals m.Id
                             where t.Id == item.Id
                             select m
                    ).FirstOrDefault();
                item.Title = movie.Title;
                var seats = (from t in _ctx.Tickets
                             join s in _ctx.SeatsBuy
                             on t.Id equals s.TicketID
                             where t.Id == item.Id
                             select s.SeatID
                  ).ToList();
                item.CountTicket = seats.Count();
            }
            vm.List = new List<RevenueMovie>();
            var listTicketGroup = list.GroupBy(a => a.Title);
            var sum = 0;
            foreach (var group in listTicketGroup)
            {
                RevenueMovie revenueMovie = new RevenueMovie();
                var title = group.Key;
                var total = 0;
                var totalTicket = 0;
                foreach (var item in group)
                {
                    total += (int)item.Total;
                    totalTicket += (int)item.CountTicket;
                }
                revenueMovie.Title = title;
                revenueMovie.Viewer = totalTicket;
                revenueMovie.TotalMoneyRevenue = total;
                sum += totalTicket;
                vm.List.Add(revenueMovie);
            }
            foreach (var item in vm.List)
            {
                item.Percent = (int)Math.Round((double)item.Viewer * 100 / sum);
            }
            return vm;
        }
        public RevenueListVm GetList(string type="",string term = "", bool paging = false, int currentPage = 1)
        {
            RevenueListVm vm = new RevenueListVm();
            var list=_ctx.Tickets.ToList();
            if(!string.IsNullOrEmpty(term))
            {
                vm.Term = term;
                DateTime date = DateTime.Parse(term);
                string datestr = date.Date.ToString("dd/MM/yyyy");
                List<Ticket> ticketList = new List<Ticket>();
                foreach(var item in list)
                {
                    DateTime datetime = (DateTime)item.DateTime;
                    if (type.Equals("day"))
                    {
                        if(datetime.Date.ToString("dd/MM/yyyy") == datestr)
                        {
                            ticketList.Add(item);
                        }
                    }
                    else
                    {
                        if (datetime.Month == date.Month)
                        {
                            ticketList.Add(item);
                        }
                    }
                }
                list=ticketList;
            }
            foreach(var item in list)
            {
                var movie = (from t in _ctx.Tickets
                             join s in _ctx.Showtimes on t.ShowtimeID equals s.ID
                             join m in _ctx.Movies on s.MovieID equals m.Id
                             where t.Id == item.Id
                             select m
                    ).FirstOrDefault();
                if (movie != null)
                {
                    item.Title = movie.Title;
                }
                var seats=(from t in _ctx.Tickets
                           join s in _ctx.SeatsBuy
                           on t.Id equals s.TicketID
                           where t.Id == item.Id
                           select s.SeatID
                  ).ToList();
                   item.CountTicket=seats.Count();
            }
            vm.List = new List<RevenueMovie>();
            var listTicketGroup=list.GroupBy(a=>a.Title);
            var sum = 0;
            foreach(var group in listTicketGroup)
            {
                RevenueMovie revenueMovie = new RevenueMovie();
                var title = group.Key;
                var total = 0;
                var totalTicket = 0;
                foreach(var item in group)
                {
                    total +=(int) item.Total;
                    totalTicket += (int)item.CountTicket;
                }
                revenueMovie.Title = title;
                revenueMovie.Viewer = totalTicket;
                revenueMovie.TotalMoneyRevenue = total;
                sum += totalTicket;
                vm.List.Add(revenueMovie);
            }
            foreach(var item in vm.List)
            {
                item.Percent =(int)Math.Round((double)item.Viewer * 100 / sum);
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = vm.List.Count();
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                vm.List = vm.List.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                vm.PageSize = pageSize;
                vm.CurrentPage = currentPage;
                vm.TotalPages = TotalPages;
            }
            return vm;
            throw new NotImplementedException();
        }
        public RevenueListVm GetTotalRevenue(bool paging = false, int currentPage = 1)
        {
            RevenueListVm vm = new RevenueListVm();
            vm.List=_ctx.Revenues.ToList();
            vm.List=vm.List.OrderByDescending(x => x.Viewer).ToList();
            foreach(var item in vm.List)
            {
                var Movie = (from m in _ctx.Movies
                                  join r in _ctx.Revenues on m.Id equals r.MovieID
                                  where r.MovieID == item.MovieID
                                  select m
                    ).FirstOrDefault();
                if(Movie != null )
                {
                    item.Title = Movie.Title;
                    item.ImagePosterMovie = Movie.ImagePosterMovie;
                }
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 6;
                int count = vm.List.Count();
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                vm.List = vm.List.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                vm.PageSize = pageSize;
                vm.CurrentPage = currentPage;
                vm.TotalPages = TotalPages;
            }
            return vm;
            throw new NotImplementedException();
        }
    }
}
