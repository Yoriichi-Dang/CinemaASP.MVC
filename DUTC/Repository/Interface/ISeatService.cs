using DUTC.Models.DTO;

namespace DUTC.Repository.Interface
{
    public interface ISeatService
    {
        bool Add(Seat model);
        bool Update(Seat model);
        bool Delete(int id);
        Seat GetSeatByID(int id);
        Seat GetSeatByName(int RoomID, string SeatID);
    }
}
