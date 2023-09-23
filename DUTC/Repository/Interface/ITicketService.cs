using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface ITicketService
    {
        Task<bool> Add(Ticket model);
        Task<TicketList> getListHistoryByID(string idUser= "", string term = "", string sortOrder = "", bool paging = false, int currentPage = 0);
        TicketListVm getTicketByID(int id);
        Task<List<Ticket>> getTicketList();
    }
}
