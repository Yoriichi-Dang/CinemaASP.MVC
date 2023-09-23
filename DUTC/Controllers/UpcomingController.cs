using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DUTC.Controllers
{
    public class UpcomingController : Controller
    {
        private readonly IMovieService _movieService;
        public UpcomingController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        public IActionResult Index(string term="")
        {
            var data=_movieService.List(term);
            data.MovieList=data.MovieList.OrderBy(x =>x.DateReleased);
            return View(data);
        }
    }
}
