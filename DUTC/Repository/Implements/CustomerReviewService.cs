using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DUTC.Repository.Implements
{
    public class CustomerReviewService : ICustomerReviewService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _ctx;
        public CustomerReviewService(ApplicationDbContext ctx, UserManager<User> userManager) 
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public bool Add(CustomerReview model)
        {
            if (model == null) { return false; }
            _ctx.CustomerReviews.Add(model);
            _ctx.SaveChanges();
            return true;
            throw new NotImplementedException();
        }

        public async Task<List<CustomerReview>> GetAll()
        {
            var list=await _ctx.CustomerReviews.ToListAsync();
            foreach (var item in list)
            {
                var user = await _userManager.FindByIdAsync(item.UserID);
                if (user != null)
                {
                    item.Name = user.Name;
                    item.ImageProfile = user.ProfilePicture;
                }
                if (item.ImageProfile.IsNullOrEmpty())
                {
                    item.ImageProfile = "avatar.jpg";
                }
            }
            return list;
            throw new NotImplementedException();
        }
    }
}
