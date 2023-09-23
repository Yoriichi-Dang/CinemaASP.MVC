using DUTC.Models.DTO;
using DUTC.Repository.Implements;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageShowtimeController : Controller
    {
        private readonly IShowtimeService _showtimeService;
        private readonly IMovieService _movieService;
        public ManageShowtimeController(IShowtimeService showtimeService, IMovieService movieService)
        {
            _showtimeService = showtimeService;
            _movieService = movieService;
        }
        public IActionResult Index(string term = "",string sortOrder="", int currentPage = 1,string search="",string type="")
        {
            term = term.Trim();
            term = term.IsNullOrEmpty() ? "" : term.ToLower();
            sortOrder = sortOrder.IsNullOrEmpty() ? "" : sortOrder.ToLower();
            ViewData["TitleOrder"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "title_asc";
            ViewData["RoomOrder"] = string.IsNullOrEmpty(sortOrder) ? "room_desc" : "room_asc";
            ViewData["DatetimeOrder"] = string.IsNullOrEmpty(sortOrder) ? "datetime_desc" : "datetime_asc";
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
                else if (sortOrder.Contains("room"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["RoomOrder"] = "room_asc";
                    }
                    else
                    {
                        ViewData["RoomOrder"] = "room_desc";
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["DatetimeOrder"] = "datetime_asc";
                    }
                    else
                    {
                        ViewData["DatetimeOrder"] = "datetime_desc";
                    }
                }
            }
            else
            {
                ViewData["DatetimeOrder"] = "datetime_asc";
            }
            var data = _showtimeService.List(term, sortOrder,true, currentPage, search,type);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Showtime model)
        {
            try
            {
                var result=_showtimeService.Update(model);
                if (result)
                {
                    return Json(new { success = true });
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
        public string FormatDate(string data)
        {
            DateTime date = DateTime.ParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return date.ToString("dd/MM/yyyy");
        }
        public IActionResult ViewShowtime(int id)
        {
            var result = _movieService.GetById(id);
            result.DateReleased = FormatDate(result.DateReleased);
            result.DateEnd = FormatDate(result.DateEnd);
            return View(result);
        }
        public IActionResult Delete(int id)
        {
            var result=_showtimeService.Delete(id);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetShowtimeNotification()
        {
            var list = _showtimeService.GetShowtimeOnDate();
            if (list != null)
            {
                return Json(new { success = true, result = list });
            }
            return Json(new { success = false });
        }
    }
}
