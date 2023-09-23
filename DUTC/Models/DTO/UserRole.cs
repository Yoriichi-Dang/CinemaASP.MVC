using DUTC.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DUTC.Models.DTO
{
    public class UserRole
    {
        public User User { get;set; }
        public List<SelectListItem> ApplicationRoles { get;set; }
    }
}
