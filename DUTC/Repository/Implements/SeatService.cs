using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using System.Security.Cryptography;

namespace DUTC.Repository.Implements
{
    public class SeatService : ISeatService
    {
        private readonly ApplicationDbContext _ctx;
        public SeatService(ApplicationDbContext ctx) { _ctx = ctx; }
        public bool Add(Seat model)
        {
            try
            {
                _ctx.Seats.Add(model);
                _ctx.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            var result=GetSeatByID(id);
            if (result != null)
            {
                _ctx.Seats.Remove(result);
                _ctx.SaveChanges();
                return true;
            }
            else return false;
            throw new NotImplementedException();
        }

        public Seat GetSeatByID(int id)
        {
            return _ctx.Seats.FirstOrDefault(x => x.Id == id);
            throw new NotImplementedException();
        }

        public bool Update(Seat model)
        {
            try
            {
                _ctx.Seats.Update(model);
                _ctx.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public Seat GetSeatByName(int RoomId, string SeatID)
        {
        
            throw new NotImplementedException();
        }

    }
}
