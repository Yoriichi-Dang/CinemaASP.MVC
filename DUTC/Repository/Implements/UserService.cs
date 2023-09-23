using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace DUTC.Repository.Implements
{
    public class UserService:IUserService
    {
        private readonly ApplicationDbContext _ctx;
        public UserService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<User>> GetRoleList(string type)
        {
            List<User> list = new List<User>();
            var usersWithRole = await _ctx.Users
    .Where(a => _ctx.UserRoles
        .Any(r => _ctx.Roles
            .Any(role => role.Id == r.RoleId && a.Id == r.UserId &&!a.Email.ToLower().StartsWith("admin")&& role.Name.ToLower().Equals(type.ToLower()))))
    .ToListAsync();

            if (usersWithRole.Count > 0)
            {
                list = usersWithRole;
            }
            return list;
        }
        public async Task<ListUserModels> GetList(string sortOrder = "",string term = "", bool paging = false, int currentPage = 0, string type = "")
        {
            ListUserModels model = new ListUserModels();
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            type = string.IsNullOrEmpty(type) ? "" : type.ToLower();
            List<User> listRole = new List<User>();
            model.Type = "all";
            List<User> list = new List<User>();
            if (!type.IsNullOrEmpty()&&!type.ToLower().Equals("all"))
            {
                listRole = await GetRoleList(type);
                model.Type = type;
            }
            else
            {
                listRole = _ctx.Users.Where(a => !a.Email.ToLower().StartsWith("admin")).ToList();
            }
            foreach (var user in listRole) {
                if (term.IsNullOrEmpty()||
    (!string.IsNullOrEmpty(user.Email) && user.Email.ToLower().StartsWith(term)) ||
    (!string.IsNullOrEmpty(user.Email) && user.Email.ToLower().EndsWith(term)) ||
    (!string.IsNullOrEmpty(user.Name) && user.Name.ToLower().StartsWith(term)) ||
    (!string.IsNullOrEmpty(user.Name) && user.Name.ToLower().EndsWith(term)) ||
    (!string.IsNullOrEmpty(user.Phone) && user.Phone.StartsWith(term)) ||
    (!string.IsNullOrEmpty(user.Phone) && user.Phone.EndsWith(term)))
                {
                    list.Add(user);
                }

            }
            if (!sortOrder.IsNullOrEmpty())
            {
                if (sortOrder.Contains("name"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.Name != null ? a.Name.Substring(a.Name.LastIndexOf(" ") + 1) : string.Empty)
            .ThenBy(a => a.Name)
            .ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.Name != null ? a.Name.Substring(a.Name.LastIndexOf(" ") + 1) : string.Empty)
           .ThenBy(a => a.Name)
           .ToList();
                    }
                }
                else if (sortOrder.Contains("birthday"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list=list.OrderByDescending(a => a.Birthday).ToList();
                    }
                    else
                    {
                        list=list.OrderBy(a => a.Birthday).ToList();
                    }
                }
                else if (sortOrder.Contains("email"))
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list = list.OrderByDescending(a => a.Email).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(a => a.Email).ToList();
                    }
                }
                else
                {
                    if (sortOrder.Contains("desc"))
                    {
                        list=list.OrderByDescending(a => a.Payment).ToList();
                    }
                    else
                    {
                        list=list.OrderBy(a => a.Payment).ToList();
                    }
                }
            }
            else
            {
                list=list.OrderByDescending(a => a.Payment).ToList();
            }
            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = list.ToList().Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                model.PageSize = pageSize;
                model.CurrentPage = currentPage;
                model.TotalPages = TotalPages;
                model.Term = term;
                model.sortOrder= sortOrder;
            }
            model.users = list.AsQueryable();
            return model;
            throw new NotImplementedException();
        }
    }
}
