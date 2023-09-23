using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface ICustomerReviewService
    {
        bool Add(CustomerReview model);
        Task<List<CustomerReview>> GetAll();
    }
}
