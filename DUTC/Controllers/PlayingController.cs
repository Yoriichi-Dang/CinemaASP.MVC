using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DUTC.Controllers
{
    public class PlayingController : Controller
    {
        private readonly IMovieService _movieService;
        public PlayingController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        public IActionResult Index(string term = "")
        {
            var data=_movieService.List(term);
            data.MovieList=data.MovieList.AsQueryable().OrderByDescending(x => x.DateReleased);
            return View(data);
        }
    }
}
