using DUTC.Data;
using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface IUserService
    {
        Task<ListUserModels> GetList(string sortOrder="",string term = "", bool paging = false, int currentPage = 0,string type="");
        Task<List<User>> GetRoleList(string type = "");

    }
}
