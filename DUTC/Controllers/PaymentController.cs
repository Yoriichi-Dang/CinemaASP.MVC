using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace DUTC.Controllers
{
    [Authorize(Roles ="User")]
    public class PaymentController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IShowtimeService _showtimeService;
        private readonly ITicketService _ticketService;
        public PaymentController(IShowtimeService showtimeService, ITicketService ticketService)
        {
            _showtimeService = showtimeService;
            _ticketService = ticketService;
        }
        [HttpPost]
        public IActionResult Index(Showtime model)
        {
            if(model.ID != 0&&model.SeatBuy.Count>0) {
                return Json(new { success = true});
            }
            return Json(new { success = false });
        }
        public IActionResult PaymentForm(int id,string SeatIDs,string total)
        {
            string totals= Encoding.UTF8.GetString(Convert.FromBase64String(total));
            string seatIDs = Encoding.UTF8.GetString(Convert.FromBase64String(SeatIDs)); // Giải mã chuỗi Base64
            List<string> seatIDList = seatIDs.Split(',').ToList();
            Ticket ticket = new Ticket();
            Showtime data= new Showtime();
            data=_showtimeService.GetById(id);
            data.movie=_showtimeService.GetMovieById(id);
            data.SeatBuy = seatIDList;
            data.Total=Convert.ToInt32(totals);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveTicket(Ticket model) {
            model.DateTime = DateTime.Now;
            if (await _ticketService.Add(model))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
