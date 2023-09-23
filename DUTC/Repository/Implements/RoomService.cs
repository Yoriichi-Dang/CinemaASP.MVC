using DUTC.Data;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DUTC.Repository.Implements
{
    public class RoomService : IRoomService
    {
        public readonly ApplicationDbContext _ctx;
        public RoomService(ApplicationDbContext ctx) { _ctx = ctx; }
        public bool Add(Room model)
        {
            try
            {
                var data=_ctx.Rooms.FirstOrDefault(r => r.RoomID == model.RoomID);
                if (data == null)
                {
                    _ctx.Rooms.Add(model);
                    _ctx.SaveChanges();
                    return true;
                }
                else return false;
                //set RoomSeat
            }
            catch (Exception ex)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public bool AddListSeat(Room model)
        {

            throw new NotImplementedException();
        }

        public bool Delete(int Id)
        {
            var result = _ctx.Rooms.Find(Id);
            if(result != null)
            {
                _ctx.Rooms.Remove(result);
                _ctx.SaveChanges();
                return true;
            }
            return false;
        }

        public Room GetByID(int Id)
        {
            return _ctx.Rooms.FirstOrDefault(a => a.Id == Id);
        }

        public List<Seat> GetListSeat(int roomID)
        {
            return null;
           //var data=_ctx.Seats.Where(a=>a.RoomID==roomID).ToList();
           // return data;
           // throw new NotImplementedException();
        }

        public RoomListVm List(string term = "", string sortOrder = "")
        {
            var data = _ctx.Rooms.AsQueryable();
            if (!term.IsNullOrEmpty())
            {
                data = data.Where(a => a.RoomID.ToString() == term);
            }
            if (!sortOrder.IsNullOrEmpty())
            {
                if (sortOrder.Contains("room"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        data=data.OrderByDescending(a => a.RoomID);
                    }
                    else
                    {
                        data=data.OrderBy(a => a.RoomID);
                    }
                }
                else if (sortOrder.Contains("row"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        data = data.OrderByDescending(a => a.Row);
                    }
                    else
                    {
                        data = data.OrderBy(a => a.Row);
                    }
                }
                else if (sortOrder.Contains("col"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        data = data.OrderByDescending(a => a.Column);
                    }
                    else
                    {
                        data = data.OrderBy(a => a.Column);
                    }
                }
                else if(sortOrder.Contains("total"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        data = data.OrderByDescending(a => a.Row * a.Column);
                    }
                    else
                    {
                        data = data.OrderBy(a => a.Row * a.Column);
                    }
                }
            }
            else
            {
                data=data.OrderBy(a => a.RoomID);
            }
            RoomListVm result=new RoomListVm();
            result.Term = term;
            result.sortOrder= sortOrder;
            result.ListRoom=data;
            return result;
            throw new NotImplementedException();
        }

        public bool Update(Room model)
        {
            throw new NotImplementedException();
        }
    }
}
