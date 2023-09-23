using DUTC.Constants;
using Microsoft.AspNetCore.Identity;

namespace DUTC.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // creating admin

            var user = new User
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Dang Hoang Nguyen",
                Phone = "0941295687",
                Payment=0,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Admin@2410");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }
    }
}
