using Microsoft.AspNetCore.Identity;

namespace DUTC.Data
{
    public class User:IdentityUser
    {
        public string? Name { get;set; }
        public string? ProfilePicture { get;set; }
        public int? Payment { get;set; }
        public string? Birthday { get;set; }
        public string? Phone { get;set; }
        public string? City { get;set; }
        public string? District { get;set; }
    }
}
