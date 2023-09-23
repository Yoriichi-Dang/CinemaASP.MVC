using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace DUTC.Repository.Implements
{
    public class ReviewMovieService : IReviewMovieService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly UserManager<User> _userManager;
        public ReviewMovieService(ApplicationDbContext ctx,UserManager<User>userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }
        public bool Add(Review model)
        {
            if (model == null) return false;
            else
            {
                _ctx.Reviews.Add(model);
                _ctx.SaveChanges();
                //update rate in revenue
                var revenueMovie = _ctx.Revenues.FirstOrDefault(a=>a.MovieID==model.MovieID);
                revenueMovie.Rate +=(int) model.Rate;
                revenueMovie.Reviewer += 1;
                _ctx.Revenues.Update(revenueMovie);
                _ctx.SaveChanges();
                return true;
            }
            throw new NotImplementedException();
        }
        public async Task<Review> GetReviewById(int id)
        {
            var movie = (from m in _ctx.Movies
                         join r in _ctx.Reviews
                         on m.Id equals r.MovieID
                         where r.Id == id
                         select m
                ).FirstOrDefault();
            Review model=new Review();
            model=_ctx.Reviews.FirstOrDefault(r => r.Id == id);
            DateTime date = (DateTime)model.TimeReview;
            model.Date = date.Date.ToString("dd/MM/yyyy");
            model.Time = date.TimeOfDay.ToString("hh\\:mm");
            var user = await _userManager.FindByIdAsync(model.UserID);
            model.UserInfo = user.Email;
            model.UserImage = user.ProfilePicture;
            model.MovieTitle = movie.Title;
            model.ImageMovie = movie.ImagePosterMovie;
            return model;
        }
        public async Task<ReviewListVm> GetList(string Title = "", string Rate = "", string sortOrder = "", bool paging = false, int currentPage = 0)
        {
            ReviewListVm vm = new ReviewListVm();
            vm.sortOrder = sortOrder;
            vm.Title = Title;
            vm.Rate = Rate;
            var data = await _ctx.Reviews.OrderByDescending(a => a.TimeReview).ToListAsync();
            if (!string.IsNullOrEmpty(Rate))
            {
                data = data.Where(a => a.Rate.ToString().Equals(Rate)).ToList();
            }
            foreach (var item in data)
            {
                var movie= (from m in _ctx.Movies
                             join r in _ctx.Reviews on m.Id equals r.MovieID
                             where r.MovieID == item.MovieID
                             select m
                ).FirstOrDefault();
                if (movie != null)
                {
                    item.ImageMovie= movie.ImagePosterMovie;
                    item.MovieTitle = movie.Title;
                }
            }
            if (!string.IsNullOrEmpty(Title))
            {
                data = data.Where(a => a.MovieTitle.Equals(Title)).ToList();
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 4;
                int count = data.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                data = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                vm.PageSize = pageSize;
                vm.CurrentPage = currentPage;
                vm.TotalPages = TotalPages;
            }
            vm.TitleMovie=_ctx.Movies.Select(a=>a.Title).ToList();
            vm.List=data.AsQueryable();
            return vm;
        }
        public async Task<ReviewListVm> GetListById(int movieId, string term, bool paging = false, int currentPage = 1)
        {
            ReviewListVm vm = new ReviewListVm();
            vm.ListRate = new List<Tuple<int, int>>();
            var data = await _ctx.Reviews.Where(a=>a.MovieID==movieId).OrderByDescending(a=>a.TimeReview).ToListAsync();
            //get list rate
             var groupedRate = data.GroupBy(a => a.Rate)
                    .Select(g => new { Rate = g.Key, Count = g.Count() })
                    .OrderByDescending(b=>b.Rate).ToList();
                foreach(var group in groupedRate)
            {
                vm.ListRate.Add(new Tuple<int, int>((int)group.Rate, group.Count));
            }
            //
            foreach (var item in data)
            {
                var movie = (from m in _ctx.Movies
                             join r in _ctx.Reviews on m.Id equals r.MovieID
                             where r.MovieID == item.MovieID
                             select m
                ).FirstOrDefault();
                if (movie != null)
                {
                    item.ImageMovie = movie.ImagePosterMovie;
                    item.MovieTitle = movie.Title;
                }
                var user=await _userManager.FindByIdAsync(item.UserID);
                item.UserInfo = user.Email;
                if (string.IsNullOrEmpty(user.ProfilePicture))
                {
                    user.ProfilePicture = "avatar.jpg";
                }
                item.UserImage = user.ProfilePicture;
                DateTime date = (DateTime)item.TimeReview;
                item.Date = date.Date.ToString("dd/MM/yyyy");
                item.Time = date.TimeOfDay.ToString("hh\\:mm");
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = data.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                data = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                vm.PageSize = pageSize;
                vm.CurrentPage = currentPage;
                vm.TotalPages = TotalPages;
            }
            vm.List = data.AsQueryable();
            return vm;
        }
    }
}
