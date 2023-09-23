using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DUTC.Controllers
{
    public class BlogController : Controller
    {
        private readonly IReviewMovieService _reviewMovieService;
        private readonly UserManager<User> _userManager;
        public BlogController(IReviewMovieService reviewMovieService, UserManager<User> userManager)
        {
            _reviewMovieService = reviewMovieService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string Title = "", string Rate = "", string sortOrder = "", bool paging = false, int currentPage = 1)
        {
            sortOrder=sortOrder.ToLower();
            var data = await _reviewMovieService.GetList(Title, Rate,sortOrder, true,currentPage); // Call the asynchronous method
            foreach (var item in data.List)
            {
                var user = await _userManager.FindByIdAsync(item.UserID);
                item.UserInfo = user.Email;
            }
            return View(data);
        }


    }
}
