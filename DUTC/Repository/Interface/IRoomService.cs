using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface IRoomService
    {
        bool Add(Room model);
        bool AddListSeat(Room model);
        bool Delete(int  roomId);
        Room GetByID(int roomId);
        bool Update(Room model);
        List<Seat>GetListSeat(int roomID);
        RoomListVm List(string term="",string sortOrder="");
        //get list seat by id room
    }
}
