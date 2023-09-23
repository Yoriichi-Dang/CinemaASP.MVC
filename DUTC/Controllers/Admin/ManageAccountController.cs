using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles ="Admin")]
    public class ManageAccountController : Controller
    {
        private  UserManager<User> _userManager;
        private readonly ILogger<ManageAccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _ctx;
        private readonly IUserService _userService;
        public ManageAccountController(ApplicationDbContext ctx, UserManager<User> userManager, ILogger<ManageAccountController>logger, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager,IUserService userService)
        {
            this._ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _userService = userService;
        }
        public async Task<IActionResult> Index(string term = "", int currentPage = 1, string sortOrder= "",string type="")
        {
            type = string.IsNullOrEmpty(type)?"all":type.ToLower();
            ViewData["NameOrder"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewData["EmailOrder"] = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "email_asc";
            ViewData["BirthOrder"] = String.IsNullOrEmpty(sortOrder) ? "birthday_desc" : "birthday_asc";
            ViewData["MoneyOrder"] = String.IsNullOrEmpty(sortOrder) ? "money_asc" : "money_desc";
            var list =await _userService.GetList(sortOrder,term, true, currentPage,type);
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
                else if (sortOrder.Contains("birthday"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["BirthOrder"] = "birthday_asc";
                    }
                    else
                    {
                        ViewData["BirthOrder"] = "birthday_desc";
                    }
                }
                else if (sortOrder.Contains("money"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["MoneyOrder"] = "money_asc";
                    }
                    else
                    {
                        ViewData["MoneyOrder"] = "money_desc";
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        ViewData["EmailOrder"] = "email_asc";
                    }
                    else
                    {
                        ViewData["EmailOrder"] = "email_desc";
                    }
                }
            }
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProfile(string id)
        {
            //Get Profile user by id
            UserRole userRole=new UserRole();
            var users = _ctx.Users.Where(x => x.Id == id).SingleOrDefault();
            var usersInRole = _ctx.UserRoles.Where(x => x.UserId == id).Select(x => x.RoleId);
            userRole.User = users;
            userRole.ApplicationRoles = await _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id,
                Selected = usersInRole.Contains(x.Id)
            }).ToListAsync();
            return View(userRole);
        }
        [HttpPost]
        public async Task<IActionResult> ViewProfile(UserRole model)
        {
            //Add Role
                var user=await _userManager.FindByIdAsync(model.User.Id);
                var selectedRoleId = model.ApplicationRoles.Where(x => x.Selected).Select(x => x.Value);//role choice
                var AlreadyExists = _ctx.UserRoles.Where(x => x.UserId == model.User.Id).Select(x => x.RoleId);//find role userID and get roleId
                var toAdd = selectedRoleId.AsEnumerable().Except(AlreadyExists);// add new roleid user when you choice new role except old role
                var toRemove = AlreadyExists.AsEnumerable().Except(selectedRoleId);//delete roleid do not select if it alreadyexist
                foreach (var item in toRemove)
                {
                    _ctx.UserRoles.Remove(new IdentityUserRole<string>
                    {
                        RoleId = item,
                        UserId = model.User.Id
                    }
                    );
                }
                foreach (var item in toAdd)
                {
                    _ctx.UserRoles.Add(new IdentityUserRole<string>
                    {
                        RoleId = item,
                        UserId = model.User.Id
                    });
                }
            _ctx.SaveChanges();// save because if do not save the db will not change
            TempData["msg"] = "Success";
            return RedirectToAction("ViewProfile", new { id=model.User.Id});
        }
        public IActionResult DeleteAccount(string id)
        {
            try
            {
              
                var result = _ctx.Users.Find(id);
                if (!User.Identity.Name.Equals(result.UserName))
                {
                    if (result != null)
                    {

                        _ctx.Users.Remove(result);
                        _ctx.SaveChanges();
                        TempData["msg"] = "Success Delete Account";
                    }
                    else
                    {
                        TempData["msg"] = "Error Delete Account";
                    }
                }
                else
                {
                    TempData["msg"] = "Can't Delete Because You Are Login Into By Your Account";
                }

            }catch(Exception ex)
            {
                return Content(ex.ToString());
            }
            return RedirectToAction("Index");
        }
    }
}
