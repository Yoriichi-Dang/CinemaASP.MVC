using DUTC.Data;
using DUTC.Models;
using DUTC.Models.DTO;
using DUTC.Repository.Implements;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;

namespace DUTC.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;
        private readonly IReviewMovieService _reviewMovieService;
        private readonly ICustomerReviewService _customerReviewService;
        private readonly ITicketService _ticketService;
        public HomeController(ILogger<HomeController> logger,IMovieService movieService, IShowtimeService showtimeService, SignInManager<User> signInManager, UserManager<User> userManager, IReviewMovieService reviewMovieService, ICustomerReviewService customerReviewService, ITicketService ticketService)
        {
            _logger = logger;
            _movieService = movieService;
            _showtimeService = showtimeService;
            _signInManager = signInManager;
            _userManager = userManager;
            _reviewMovieService = reviewMovieService;
            _customerReviewService = customerReviewService;
            _ticketService = ticketService;
        }

        public IActionResult Index()
        {
            var data = new IndexClass();
            //get model MovieListVm
            var model = _movieService.List();
            model.MovieList = model.MovieList.AsQueryable().OrderByDescending(a => a.DateReleased);
            //end
            data.MovieListVm = model;//set MovieListVm to Indexclass
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetReviewService() 
        {
            var result = await _customerReviewService.GetAll();
            if(result.Count > 0)
            {
                return Json(new { success = true, result = result });
            }
            else
            {
                return Json(new { success = false, });
            }
        }
        public IActionResult Service()
        {
            return View();
        }
        public string FormatDate(string data)
        {
            DateTime date = DateTime.ParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return date.ToString("dd/MM/yyyy");
        }
        [HttpGet]
        public IActionResult SearchMovie(string term="",string date="")
        {
            var result = _movieService.GetListShowtimeByTitle(term, date);
            if(result.Count() > 0)
            {
                return Json(new {success=true,data=result});
            }
            return Json(new { success = false });
        }
        public IActionResult MovieDetails(int id)
        {
            var result = _movieService.GetById(id);
            result.DateReleased = FormatDate(result.DateReleased);
            result.DateEnd = FormatDate(result.DateEnd);
            return View(result);
        }
        [HttpGet]
        public IActionResult GetShowtimeMovie(int id,string term="",int currentPage=1)
        {
            var showtime=_movieService.GetListShowtimeById(id, term,true,currentPage);
            if (showtime != null)
            {
                return Json(new { success = true,data=showtime });
            }
            else
            {
                return Json(new {success=false});
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewTicket(int id)
        {
            TicketListVm bill = _ticketService.getTicketByID(id);
            var user = await _userManager.FindByIdAsync(bill.ticket.UserID);
            if (user != null)
            {
                bill.ticket.UserInfo = user.Email;
            }
            return View(bill);
        }
        [HttpGet]
        public async Task<ActionResult> GetReviewList(int id,string term="",int currentPage=1)
        {
            var data=await _reviewMovieService.GetListById(id,term,true,currentPage);
            if (data != null)
            {
                return Json(new { success = true, result = data });

            }
            else return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult Room(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                Showtime model = _showtimeService.GetById(id);
                model.room = _showtimeService.GetRoombyId(id);
                model.seats = _showtimeService.getSeats(id);
                model.movie = _showtimeService.GetMovieById(id);
                if (model != null && model.room != null && model.movie != null && model.seats.Count > 0)
                {
                    return View(model);
                }
                return RedirectToAction(nameof(MovieDetails), new { id = model.MovieID });
            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddReviewService(string content)
        {
           CustomerReview model=new CustomerReview();
            model.Content = content;
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                model.UserID = currentUser.Id;
                model.Time= DateTime.Now;
                if (_customerReviewService.Add(model))
                {
                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetListShowtimePlaying()
        {
             
            DateTime currentDate = DateTime.Now;
            //get model MovieListVm
            var model = _movieService.List();
            var data = model.MovieList.OrderByDescending(a => a.Viewer).ToList();
            for (int i = data.Count() - 1; i >= 0; i--)
            {
                var item = data[i];
                DateTime dateReleased = DateTime.ParseExact(item.DateReleased, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (DateTime.Compare(dateReleased, currentDate) > 0)
                {
                    data.RemoveAt(i);
                }
            }
            model.MovieList = data.AsQueryable();
            //end
            if (model.MovieList == null)
            {
                return Json(new { success = false});
            }
            return Json(new {success=true, result= model });
        }
        public async Task<IActionResult> AddReviewMovie(Review model)
        {
           bool result= _reviewMovieService.Add(model);
            if (result)
            {
                model =await _reviewMovieService.GetReviewById(model.Id);
                return Json(new { success=true ,result=model});
            }
            else
            {
                return Json(new {success=false});
            }
        }
        
    }
}