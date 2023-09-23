using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface IRevenueMovieService
    {
        RevenueListVm GetList(string type="",string term = "", bool paging = false, int currentPage = 1);
        RevenueListVm GetTotalRevenue(bool paging = false, int currentPage = 1);
        Tuple<int,double> GetMonthEarning(string date1 = "", string date2 = "");
        RevenueListVm GetYearEarning(string Year = "");

    }
}
