// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DUTC.Data;
using DUTC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DUTC.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFileService _fileService;
        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IFileService fileService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._fileService = fileService;
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
            public string? Phone { get; set; }
            public string? Name { get; set; }
            public string? District { get;set; }
            public string? City { get; set; }
            public int? Payment { get; set; }
            public string? Birthday { get;set; }
            public string? ProfilePicture { get;set; }
            public IFormFile ImageFile { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                //PhoneNumber = phoneNumber,
                Phone = user.Phone,
                Name = user.Name,
                District = user.District,
                City = user.City,
                Birthday=user.Birthday,
                Payment=(int)user.Payment,
                ProfilePicture=user.ProfilePicture
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.Phone != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Phone);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (user.Name == null||user.Name!=Input.Name)
            {
                user.Name=Input.Name;
                await _userManager.UpdateAsync(user);
            }
            if (user.District == null || user.District != Input.District)
            {
                user.District = Input.District;
                await _userManager.UpdateAsync(user);
            }  if (user.City == null || user.City != Input.City)
            {
                user.City = Input.City;
                await _userManager.UpdateAsync(user);
            }
            if (user.Birthday == null || user.Birthday != Input.Birthday)
            {
                user.Birthday = Input.Birthday;
                await _userManager.UpdateAsync(user);
            }
            if (user.Payment == null)
            {
                user.Payment = 0;
                await _userManager.UpdateAsync(user);
            }
            StatusMessage = "Your profile has been updated";
            if (Input.ImageFile != null)
            {
                var result = _fileService.SaveImage(Input.ImageFile);
                if (result.Item1 == 1)
                {
                    var oldImage = user.ProfilePicture;
                    user.ProfilePicture = result.Item2;
                    await _userManager.UpdateAsync(user);
                    var deleteResult = _fileService.DeleteImage(oldImage);
                }
                else
                {
                    StatusMessage = "Error when trying to add Image.";
                    return RedirectToPage();
                }
            }
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }
    }
}
