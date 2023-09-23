using DUTC.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using DUTC.Areas.Identity.Pages.Account;

namespace DUTC.Repository.Implements
{
    public class AccountRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        public AccountRepository(UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        public async Task<User> changedRole(string id)
        {
            var result = await _userManager.FindByIdAsync(id);

            if (result == null)
            {
                throw new Exception($"User with ID {id} not found");
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(result, "Admin");

            if (!addToRoleResult.Succeeded)
            {
                throw new Exception($"Failed to add user with ID {id} to role");
            }

            return result;
        }

    }
}
