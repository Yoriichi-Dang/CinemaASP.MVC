using DUTC.Constants;
using DUTC.Models.DTO;
using DUTC.Repository.Implements;
using DUTC.Repository.Interface;
using DUTC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        private readonly IShowtimeService _showtimeService;
        public MovieController(IMovieService movieService, IFileService fileService, IGenreService genreService,IShowtimeService showtimeService)
        {
            _movieService = movieService;
            _fileService = fileService;
            _genreService = genreService;
            _showtimeService = showtimeService;
        }
        public IActionResult Index(string type = "",string sortOrder="" ,string term = "", int currentPage = 1)
        {
            term = term.Trim();
            term = term.IsNullOrEmpty() ? "" : term.ToLower();
            sortOrder = sortOrder.IsNullOrEmpty() ? "" : sortOrder.ToLower();
            ViewData["TitleOrder"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "title_asc";
            ViewData["NationOrder"] = string.IsNullOrEmpty(sortOrder) ? "nation_desc" : "nation_asc";
            ViewData["ViewertimeOrder"] = string.IsNullOrEmpty(sortOrder) ? "viewer_desc" : "viewer_asc";
            ViewData["RateOrder"] = string.IsNullOrEmpty(sortOrder) ? "rate_desc" : "rate_asc";

            if (!sortOrder.IsNullOrEmpty())
            {
                if (sortOrder.Contains("title"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["TitleOrder"] = "title_asc";
                    }
                    else
                    {
                        ViewData["TitleOrder"] = "title_desc";
                    }
                }
                else if (sortOrder.Contains("nation"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["NationOrder"] = "nation_asc";
                    }
                    else
                    {
                        ViewData["NationOrder"] = "nation_desc";
                    }
                }
                else if (sortOrder.Contains("viewer"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["ViewerOrder"] = "viewer_asc";
                    }
                    else
                    {
                        ViewData["ViewerOrder"] = "viewer_desc";
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["RateOrder"] = "rate_asc";
                    }
                    else
                    {
                        ViewData["RateOrder"] = "rate_desc";
                    }
                }
            }
            else
            {
                ViewData["ViewerOrder"] = "viewer_asc";
            }
            var data = this._movieService.List(type, sortOrder,term, true, currentPage);
            return View(data);
        }
        public IActionResult Add()
        {
            var model = new Movie();
            model.GenreList = _genreService.List().Genres.Select(a => new SelectListItem
            {
                Text = a.GenreName,
                Value = a.Id.ToString()
            });
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Movie model)
        {
            //fix bug 2 ngày chỉ vì video file full hd 1080 quá dung lượng của hệ thống cho vào
            model.GenreList = _genreService.List().Genres.Select(a => new SelectListItem
            {
                Text = a.GenreName,
                Value = a.Id.ToString()
            });
            if (model.Imagefile != null && model.Videofile != null)
            {
                var fileresult1 = this._fileService.SaveImage(model.Imagefile);
                if (fileresult1.Item1 == 0)
                {
                    TempData["msg"] = "Error could not saved file Movie";
                    return View(model);
                }
                var imageFile = fileresult1.Item2;
                model.ImagePosterMovie = imageFile;
                //video
                var fileresult2 = this._fileService.SaveImage(model.Videofile);
                if (fileresult2.Item1 == 0)
                {
                    TempData["msg"] = "Error could not saved file Video";
                    return View(model);
                }
                var videoFile = fileresult2.Item2;
                model.VideoPosterMovie = videoFile;
            }
                var result = _movieService.Add(model);
                if (result)
                {
                    TempData["msg"] = "Successfully Add New Movie";
                    return RedirectToAction(nameof(Add));
                }
                else
                {
                    TempData["msg"] = "Error because Movie has been exist!";
                    //error
                    return View(model);
                }
        }
        public IActionResult CopyShowtime(int id)
        {
            var result = _showtimeService.MakeCopyShowtimeMovie(id);
            return RedirectToAction(nameof(MovieDetail), new {id=id});
        }
        public string FormatDate(string data)
        {
            DateTime date = DateTime.ParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return date.ToString("dd/MM/yyyy");
        }
        public IActionResult MovieDetail(int id)
        {
            var result = _movieService.GetById(id);
            result.DateReleased=FormatDate(result.DateReleased);
            result.DateEnd=FormatDate(result.DateEnd);
            return View(result);
        }
        public IActionResult Edit(int id)
        {
            var model=_movieService.GetById(id);
            var selectedGenres=_movieService.GetGenreByMovieId(id);
            MultiSelectList multigenresList = new MultiSelectList(_genreService.List().Genres, "Id", "GenreName", selectedGenres);
            model.MultiGenreList = multigenresList;
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(Movie model)
        {
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);
            MultiSelectList multigenresList = new MultiSelectList(_genreService.List().Genres, "Id", "GenreName", selectedGenres);
            model.MultiGenreList= multigenresList;
            if (model.Imagefile != null && model.Videofile != null)
            {
                var fileresult1 = this._fileService.SaveImage(model.Imagefile);
                if (fileresult1.Item1 == 0)
                {
                    TempData["msg"] = "Error could not saved file Movie";
                    return View(model);
                }
                var imageFile = fileresult1.Item2;
                model.ImagePosterMovie = imageFile;
                //video
                var fileresult2 = this._fileService.SaveImage(model.Videofile);
                if (fileresult2.Item1 == 0)
                {
                    TempData["msg"] = "Error could not saved file Video";
                    return View(model);
                }
                var videoFile = fileresult2.Item2;
                model.VideoPosterMovie = videoFile;
            }
                var result = _movieService.Update(model);
                if (result)
                {
                    return RedirectToAction(nameof(MovieDetail), new { id = model.Id });
                }
                else
                {
                    TempData["msg"] = "Error because Movie has been exist!";
                    //error
                    return View(model);
                }
        }
        [HttpPost]
        public IActionResult Update(Movie model)
        {
            if (ModelState.IsValid)
            {
                var result = _movieService.Update(model);
                if (result)
                {
                    TempData["msg"] = "Update Genre Successfully!";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
                else
                {
                    TempData["msg"] = "Error Update Genre";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
            }
            else
            {
                return RedirectToAction(nameof(Edit));
            }
        }
        [HttpGet]
        public IActionResult MovieList(string type = "",string sortOrder="", string term = "", int currentPage = 1)
        {
            var data = this._movieService.List(type, sortOrder, term, true, currentPage);
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var result = _movieService.Delete(id);
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public ActionResult AddShowtime(Showtime model)
        {
            try
            {
                var result = _showtimeService.Add(model);
                if (result)
                {
                    return Json(new { success = true});
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false });
            }
        }
    }
}
