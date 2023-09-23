using DUTC.Constants;
using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageTicket : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly UserManager<User> _userManager;
        public ManageTicket(ITicketService ticketService,UserManager<User>userManager)
        {
            _ticketService = ticketService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string term = "", string sortOrder="",string userGmail = "", int currentPage = 1)
        {
            term=term.Trim();
            term=term.IsNullOrEmpty()?"":term.ToLower();
            sortOrder=sortOrder.IsNullOrEmpty()?"":sortOrder.ToLower();
            ViewData["NameOrder"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewData["timeOrder"] = string.IsNullOrEmpty(sortOrder) ? "timepay_desc" : "timepay_asc";
            ViewData["MoneyOrder"] = string.IsNullOrEmpty(sortOrder) ? "total_desc" : "total_asc";
            TicketList list = await _ticketService.getListHistoryByID(userGmail,term,sortOrder,true,currentPage);
            if (!sortOrder.IsNullOrEmpty())
            {
                if (sortOrder.Contains("name"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["NameOrder"] = "name_asc";
                    }
                    else
                    {
                        ViewData["NameOrder"] = "name_desc";
                    }
                }
                else if (sortOrder.Contains("timepay"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["timeOrder"] = "timepay_asc";
                    }
                    else
                    {
                        ViewData["timeOrder"] = "timepay_desc";
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["MoneyOrder"] = "total_asc";
                    }
                    else
                    {
                        ViewData["MoneyOrder"] = "total_desc";
                    }
                }
            }
            else
            {
                ViewData["timeOrder"] = "timepay_asc";
            }
            return View(list);
        }
        public async Task<IActionResult> ViewBill(int id)
        {
            TicketListVm bill=_ticketService.getTicketByID(id);
            var user = await _userManager.FindByIdAsync(bill.ticket.UserID);
            if (user != null)
            {
                bill.ticket.UserInfo = user.Email;
            }
            return View(bill);
        }
    }
}
