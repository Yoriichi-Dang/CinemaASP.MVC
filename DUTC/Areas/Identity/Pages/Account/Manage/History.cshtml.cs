using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using DUTC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace DUTC.Areas.Identity.Pages.Account.Manage
{
    public class HistoryModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITicketService _ticketService;
        public HistoryModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITicketService ticketService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
           _ticketService = ticketService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string Name { get; set; }
            public string ProfilePicture { get; set; }
            public TicketList TicketList { get; set; }
        }

        private async Task LoadAsync(User user, string term = "",string sortOrder="", int currentPage = 1)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                //PhoneNumber = phoneNumber,
                Name = user.Name,
                ProfilePicture = user.ProfilePicture,
                TicketList =await _ticketService.getListHistoryByID(user.Id,term,sortOrder,true,currentPage)
            };
        }

        public async Task<IActionResult> OnGetAsync(string term = "", string sortOrder = "", int currentPage = 1)
        {
            var user = await _userManager.GetUserAsync(User);//get profile user
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user, term, sortOrder, currentPage);
            return Page();
        }

    }
}
