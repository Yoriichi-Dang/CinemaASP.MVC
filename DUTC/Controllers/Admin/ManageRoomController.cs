using DUTC.Models.DTO;
using DUTC.Repository.Implements;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageRoomController : Controller
    {
        private readonly IRoomService _roomService;
        public ManageRoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        public IActionResult Index(string term="",string sortOrder="")
        {
            term=string.IsNullOrEmpty(term)?"":term.ToLower();
            sortOrder = sortOrder.IsNullOrEmpty() ? "" : sortOrder.ToLower();
            ViewData["RoomOrder"] = sortOrder.IsNullOrEmpty() ? "room_desc" : "room_asc";
            ViewData["RowOrder"] = sortOrder.IsNullOrEmpty() ? "row_desc" : "row_asc";
            ViewData["ColOrder"] = sortOrder.IsNullOrEmpty() ? "col_desc" : "col_asc";
            ViewData["TotalOrder"] = sortOrder.IsNullOrEmpty() ? "total_asc" : "total_asc";
            if (!sortOrder.IsNullOrEmpty())
            {
                if (sortOrder.Contains("room"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        ViewData["RoomOrder"] = "room_asc";
                    }
                    else
                    {
                        ViewData["RoomOrder"] = "room_desc";
                    }
                }
                else if (sortOrder.Contains("row"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        ViewData["RowOrder"] = "row_asc";
                    }
                    else
                    {
                        ViewData["RowOrder"] = "row_desc";
                    }
                }
                else if (sortOrder.Contains("col"))
                {
                    if (sortOrder.Contains("des"))
                    {
                        ViewData["ColOrder"] = "col_asc";
                    }
                    else
                    {
                        ViewData["ColOrder"] = "col_desc";
                    }
                }
                else
                {
                    if (sortOrder.Contains("des"))
                    {
                        ViewData["TotalOrder"] = "total_asc";
                    }
                    else
                    {
                        ViewData["TotalOrder"] = "total_desc";
                    }
                }
            }
            else
            {
                ViewData["RoomOrder"] = "room_desc";
            }
            return View(_roomService.List(term,sortOrder));
        }
        [HttpPost]
        public ActionResult Add(Room model)
        {
            try
            {
                var result = _roomService.Add(model);
                if (result)
                {
                    TempData["msg"] = "Successfully Add New Room";
                    return Json(new { success = true});
                }
                else
                {
                    TempData["msg"] = "Error Add New Room";
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false });
            }
        }

        public IActionResult RoomDetail(int id)
        {
            var data = _roomService.GetByID(id);
            try
            {
                if (data == null)
                {
                    TempData["msg"] = "Error don't have RoomID: " + id;
                    return View("Not Found RoomID: " + id);
                }
                else
                {
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                return View("Not Found RoomID: " + id);
            }
        
        }
        public IActionResult Delete(int id)
        {
            var result=_roomService.Delete(id);
            if (result)
            {
                TempData["msg"] = "Successfully Delete RoomID: " + id;

            }
            else
            {
                TempData["msg"] = "Error Delete RoomID: " + id;
            }
            return RedirectToAction("Index");
        }
    }
}
