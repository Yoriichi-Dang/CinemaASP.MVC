using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using System.Linq;
using System;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Net.NetworkInformation;
namespace DUTC.Repository.Implements
{
    public class MovieService : IMovieService
    {
        public readonly ApplicationDbContext _ctx;
        public MovieService(ApplicationDbContext ctx) {  _ctx = ctx; }
        public bool Add(Movie model)
        {
            try
            {

                _ctx.Movies.Add(model);
                _ctx.SaveChanges();
                RevenueMovie revenue = new RevenueMovie();
                revenue.MovieID = model.Id;
                revenue.Rate = 0;
                revenue.TotalMoneyRevenue = 0;
                revenue.Viewer = 0;
                _ctx.Revenues.Add(revenue);
                _ctx.SaveChanges();
                foreach(int genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    _ctx.MoviesGenres.Add(movieGenre);
                }
                _ctx.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                    return false;
                var movieGenres = _ctx.MoviesGenres.Where(a => a.MovieId == data.Id);
                foreach (var movieGenre in movieGenres)
                {
                    _ctx.MoviesGenres.Remove(movieGenre);
                }
                //remove showtime
                var showtimeList = _ctx.Showtimes.Where(a => a.MovieID == data.Id);
                foreach (var showtime in showtimeList)
                {
                    _ctx.Showtimes.Remove(showtime);
                }
                //end
                _ctx.Movies.Remove(data);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public Movie GetById(int id)
        {
            var data = _ctx.Movies.Find(id);
            var genres = (from genre in _ctx.Genres
                          join mg in _ctx.MoviesGenres
                          on genre.Id equals mg.GenreId
                          where mg.MovieId == data.Id
                          select genre.GenreName
                                 ).ToList();
            var genreNames = string.Join(',', genres);
            data.GenreNames = genreNames;
            //showtime
            var showtimes =_ctx.Showtimes.Where(a=>a.MovieID ==id).OrderBy(a=>a.Time).ToList();
            if(showtimes.Count > 0 )
            {
                data.ListShowtimes = showtimes;
            }
            else
            {
                data.ListShowtimes = null;
            }
            //revenue
            var revenue = (from r in _ctx.Revenues
                           join m in _ctx.Movies
                           on r.MovieID equals m.Id
                           where m.Id == id
                           select r
                            ).FirstOrDefault();
            if (revenue != null)
            {
                data.Viewer = revenue.Viewer;
                data.TotalRate = revenue.Rate;
                if (revenue.Reviewer != 0)
                {
                    double rate = (double)revenue.Rate / revenue.Reviewer;
                    data.RateMovie = Math.Round(rate, 1);
                    data.Reviewer=revenue.Reviewer;
                }
                else
                {
                    data.RateMovie = revenue.Rate;
                    revenue.Reviewer = 1;
                    data.Reviewer = 1;
                    _ctx.Revenues.Update(revenue);
                    _ctx.SaveChanges();
                }
            }
            //
            return data;
            throw new NotImplementedException();
        }
        public ListShowtimeMovieVm GetListShowtimeById(int movieId,string term="", bool paging = false, int currentPage = 0)
        {
            ListShowtimeMovieVm resultList=new ListShowtimeMovieVm();
            // Khai báo mảng List hai chiều
            List<ShowtimeMovieVm> result = new List<ShowtimeMovieVm>();
            DateTime currentDate=DateTime.Now;
            // Duyệt qua các phần tử trong groupedObjects và thêm vào mảng List hai chiều
            var showtimes = _ctx.Showtimes.Where(a => a.MovieID == movieId).OrderBy(a => a.Time).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                DateTime date=DateTime.Parse(term);
                showtimes=showtimes.Where(a=>a.Time.Date.ToString("dd/MM/yyyy")==date.Date.ToString("dd/MM/yyyy")).ToList();
            }
            if (showtimes.Count() > 0)
            {
                var groupedObjects = showtimes.AsQueryable().Where(a => a.Time >= DateTime.Now).GroupBy(a => a.Time.Date).Select(g => new
                {
                    Date = g.Key.ToString("dd/MM/yyyy"),
                    Times = g.Select(dt => new { Time = dt.Time.TimeOfDay.ToString("hh\\:mm"), Id = dt.ID }).ToList()
                });
                foreach (var group in groupedObjects)
                {
                    var item = new ShowtimeMovieVm();
                    item.Date = group.Date;
                    item.Time = new List<Tuple<int, string>>(); // Khởi tạo List cho thuộc tính Time
                    foreach (var time in group.Times)
                    {
                        Tuple<int, string> value = new Tuple<int, string>(time.Id, time.Time);
                        item.Time.Add(value);
                    }
                    result.Add(item);
                }
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 3;
                int count = result.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                result = result.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                resultList.PageSize = pageSize;
                resultList.CurrentPage = currentPage;
                resultList.TotalPages = TotalPages;
            }
            resultList.List = result;
            return resultList;
            // Bây giờ bạn có một mảng List hai chiều chứa dữ liệu từ groupedObjects
        }
        public List<ShowtimeMovieVm> GetListShowtimeByTitle(string term = "", string date = "")
        {
            term=term.Trim();
            term = term.ToLower();
            List<ShowtimeMovieVm> result = new List<ShowtimeMovieVm>();

            // Duyệt qua các phần tử trong groupedObjects và thêm vào mảng List hai chiều
          
                var showtimes = (from s in _ctx.Showtimes
                                 join m in _ctx.Movies
                                 on s.MovieID equals m.Id
                                 where m.Title.ToLower().Trim().Equals(term.ToLower())
                                 select s
               ).OrderBy(a => a.Time).ToList();
            if(showtimes.Count() == 0&&string.IsNullOrEmpty(term)) {
                showtimes=_ctx.Showtimes.OrderBy(a => a.Time).ToList();
            }
              if (!string.IsNullOrEmpty(date))
            {
                DateTime time = DateTime.Parse(date);
                string timestr = time.ToString("HH:mm");
                string datestr = time.Date.ToString("dd/MM/yyyy");
                showtimes = showtimes.Where(a => a.Time.Date.ToString("dd/MM/yyyy") == datestr).ToList();
                var r = showtimes.Where(a => a.Time.ToString("HH:mm") == timestr).ToList();
                if (r.Any())//tim khong ra
                {
                    showtimes = r;
                }
                else
                {
                    if (showtimes.Count() > 0)
                    {
                        int index = 0;
                        TimeSpan diff = showtimes.ElementAt(index).Time.TimeOfDay - time.TimeOfDay;
                        while (diff.TotalSeconds < 0&&index<showtimes.Count()-1)
                        {
                            diff = showtimes.ElementAt(++index).Time.TimeOfDay - time.TimeOfDay;
                        }
                        List<Showtime> b = new List<Showtime>();
                        b.Add(showtimes.ElementAt(index));
                        if (index > 0)
                        {
                            b.Add(showtimes.ElementAt(index-1));
                        }
                        b=b.OrderBy(a=>a.Time.TimeOfDay).ToList();
                        showtimes = b;
                    }
                }
            }
            if (showtimes.Count() > 0)
            {
                var groupedObjects = showtimes.AsQueryable().Where(a => a.Time >= DateTime.Now).GroupBy(a => a.Time.Date).Select(g => new
                {
                    Date = g.Key.ToString("dd/MM/yyyy"),
                    Times = g.Select(dt => new { Time = dt.Time.TimeOfDay.ToString("hh\\:mm"), Id = dt.ID }).ToList()
                });
                foreach (var group in groupedObjects)
                {
                    var item = new ShowtimeMovieVm();
                    item.Date = group.Date;
                    item.Time = new List<Tuple<int, string>>(); // Khởi tạo List cho thuộc tính Time
                    foreach (var time in group.Times)
                    {
                        Tuple<int, string> value = new Tuple<int, string>(time.Id, time.Time);
                        item.Time.Add(value);
                    }
                    result.Add(item);
                }
            }
            return result;
        }
        public List<int> GetGenreByMovieId(int movieId)
        {
            var genreIds = _ctx.MoviesGenres.Where(a => a.MovieId == movieId).Select(a => a.GenreId).ToList();
            return genreIds;
        }

        public MovieListVm List(string type="",string sortOrder = "", string term = "", bool paging = false, int currentPage = 0)
        {
           
                var data = new MovieListVm();

                var list = _ctx.Movies.OrderBy(movie=>movie.Nation).ToList();
                foreach (var movie in list)
                {
                    var genres = (from genre in _ctx.Genres
                                  join mg in _ctx.MoviesGenres
                                  on genre.Id equals mg.GenreId
                                  where mg.MovieId == movie.Id
                                  select genre.GenreName
                                  ).ToList();
                    var genreNames = string.Join(',', genres);
                    movie.GenreNames = genreNames;
                //review blog
                var highestRatedReview = _ctx.Reviews
                    .Where(a => a.MovieID == movie.Id)
                     .OrderByDescending(b => b.Rate)
                    .FirstOrDefault();
                if(highestRatedReview != null)
                {
                    movie.ContentReview = highestRatedReview.ContentReview;
                }

                //revenue
                var revenue = (from r in _ctx.Revenues
                               join m in _ctx.Movies
                               on r.MovieID equals m.Id
                               where m.Id == movie.Id
                               select r
                             ).FirstOrDefault();
                if( revenue != null)
                {
                    movie.TotalRate = revenue.Rate;
                    movie.Viewer = revenue.Viewer;
                    if (revenue.Reviewer != 0)
                    {
                        double rate = (double)revenue.Rate / revenue.Reviewer;
                        movie.RateMovie = Math.Round(rate, 1);
                        movie.Reviewer=revenue.Reviewer;
                    }
                    else
                    {
                        movie.RateMovie = 10;
                        revenue.Reviewer = 1;
                        movie.Reviewer = revenue.Reviewer;
                        _ctx.Revenues.Update(revenue);
                        _ctx.SaveChanges();
                    }
                }
                else
                {
                    RevenueMovie revenueMovie = new RevenueMovie();
                    revenueMovie.MovieID = movie.Id;
                    revenueMovie.Viewer = 0;
                    revenueMovie.Rate = 10;
                    _ctx.Revenues.Add(revenueMovie);
                    _ctx.SaveChanges();
                }
             }
            if (!string.IsNullOrEmpty(type)&&!type.Equals("All"))
            {
                if (type.Equals("Upcoming"))
                {
                    list =list.Where(a=>DateTime.Parse(a.DateReleased)>DateTime.Now).ToList();
                }
                else
                {
                    list = list.Where(a => DateTime.Parse(a.DateReleased) <= DateTime.Now).ToList();
                }
                data.Type = type;
            }
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data.Term = term;
                var result = list.Where(a => a.Title.ToLower().StartsWith(term) || a.Nation.ToLower().StartsWith(term)||a.GenreNames.ToLower().Contains(term.ToLower())).ToList();
                if (result.Count > 0)
                {
                    list = result;
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
                else if (sortOrder.Contains("nation"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.Nation).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.Nation).ToList();
                    }
                }
                else if (sortOrder.Contains("viewer"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.Viewer).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.Viewer).ToList();
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.RateMovie).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.RateMovie).ToList();
                    }
                }
            }
            else
            {
                list = list.OrderByDescending(a => a.Viewer).ToList();
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPages = TotalPages;
            }
            data.sortOrder = sortOrder;
            data.MovieList = list.AsQueryable();
            return data;
        }
       
        public bool Update(Movie model)
        {
            try
            {
                // these genreIds are not selected by users and still present is movieGenre table corresponding to
                // this movieId. So these ids should be removed.
                var genresToDeleted = _ctx.MoviesGenres.Where(a => a.MovieId == model.Id && !model.Genres.Contains(a.GenreId)).ToList();
                foreach (var mGenre in genresToDeleted)
                {
                    _ctx.MoviesGenres.Remove(mGenre);
                }
                foreach (int genId in model.Genres)
                {
                    var movieGenre = _ctx.MoviesGenres.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genId);
                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genId, MovieId = model.Id };
                        _ctx.MoviesGenres.Add(movieGenre);
                    }
                }

                _ctx.Movies.Update(model);
                // we have to add these genre ids in movieGenre table
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
