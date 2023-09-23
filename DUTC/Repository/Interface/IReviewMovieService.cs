using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface IReviewMovieService
    {
        bool Add(Review model);
        Task<ReviewListVm> GetList(string Title="",string Rate="",string sortOrder = "", bool paging = false, int currentPage = 0);
        Task<ReviewListVm> GetListById(int movieId,string term,bool paging=false,int currentPage=1);
        Task<Review> GetReviewById(int id);
    }
}
