using DUTC.Models.DTO;
using DUTC.Repository.Implements;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRevenueMovieService _revenueMovieService;
        private readonly ITicketService _ticketService;
        public AdminController(IRevenueMovieService revenueMovieService, ITicketService ticketService)
        {
            _revenueMovieService = revenueMovieService;
            _ticketService = ticketService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetTotal(int currentPage = 1)
        {
            var r= _revenueMovieService.GetTotalRevenue(true,currentPage);
            if (r != null)
            {
                return Json(new {success=true,result=r});
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetTicketSales(string type="",string date="",int currentPage = 1)
        {
            var r = _revenueMovieService.GetList(type,date,true, currentPage);
            if (r != null)
            {
                return Json(new { success = true, result = r });
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetMonthlyEarning(string date1="",string date2="")
        {
            if (string.IsNullOrEmpty(date1) && string.IsNullOrEmpty(date2))
            {
                date1 = DateTime.Now.ToString();
                DateTime currentDate = DateTime.Now;
                DateTime previousMonth = currentDate.AddMonths(-1);
                date2=previousMonth.ToString();
            }
            var r= _revenueMovieService.GetMonthEarning(date1,date2);
            if (r != null)
            {
                return Json(new { success = true, result = r });
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetYearlyEarning(string year="",string year2="")
        {
            string dateCurrent = "";
            string datePrevious = "";
            if (string.IsNullOrEmpty(year) && string.IsNullOrEmpty(year2))
            {
                DateTime datetime =DateTime.Now;
                dateCurrent=datetime.Year.ToString();
                datePrevious=datetime.AddYears(-1).Year.ToString();
            }
            var r = _revenueMovieService.GetYearEarning(dateCurrent);
            var m = _revenueMovieService.GetYearEarning(datePrevious);
            if (r != null)
            {
                var totalYearCurrent = 0;
                var totalYearPrevious = 0;
                foreach(var item in r.List)
                {
                    totalYearCurrent += item.TotalMoneyRevenue;
                }
                foreach(var item in m.List)
                {
                    totalYearPrevious += item.TotalMoneyRevenue;
                }
                double denta = 0;
                double roundedDenta = 0;
                if (totalYearPrevious != 0)
                {
                    denta = totalYearCurrent * 100 / totalYearPrevious;
                    roundedDenta = Math.Round(denta, 1);
                    roundedDenta -= 100;
                }
                Tuple<int, double> data = new Tuple<int, double>(totalYearCurrent, roundedDenta);
                return Json(new { success = true, result = data });
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public async Task<IActionResult> GetSystemPayment()
        {
            var list=await _ticketService.getTicketList();
            if(list != null)
            {
                return Json(new { success = true, result = list });
            }
            return Json(new { success = false });
        }
    }
}
