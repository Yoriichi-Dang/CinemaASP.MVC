using DUTC.Constants;
using DUTC.Models.DTO;
using DUTC.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DUTC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]

    public class ManageGenreController : Controller
    {
        private readonly IGenreService _genreService;
        public ManageGenreController(IGenreService genreService)
        {
            this._genreService = genreService;
        }
        public IActionResult Index(string term = "", string sortOrder = "")
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder.ToLower();
            var result = _genreService.List(sortOrder,term);
            ViewData["NameOrder"] = sortOrder.IsNullOrEmpty() ? "name_desc" : "name_asc";
            ViewData["IDOrder"] = sortOrder.IsNullOrEmpty() ? "id_desc" : "id_asc";
            if (sortOrder.Contains("id"))
            {
                if (sortOrder.Contains("desc"))
                {
                    ViewData["IDOrder"] = "id_asc";
                }
                else
                {
                    ViewData["IDOrder"] = "id_desc";
                }
            }
            else if (sortOrder.Contains("name"))
            {
                if (sortOrder.Contains("desc"))
                {
                    ViewData["NameOrder"] = "name_asc";
                }
                else
                {
                    ViewData["NameOrder"] = "name_desc";
                }
            }
            return View(result);
        }
        public IActionResult Add()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Add(Genre model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["msg"] = "Error";
        //        return View(model);
        //    }
        //    else
        //    {
        //        var result = _genreService.Add(model);
        //        if (result)
        //        {
        //            TempData["msg"] = "Successfully Add New Genre";
        //            return RedirectToAction(nameof(Add));
        //        }
        //        else
        //        {
        //            TempData["msg"] = "Error because Genre has been exist!";
        //            return View();
        //        }
        //    }
        //}
        [HttpPost]
        public ActionResult Add(Genre model)
        {
            try
            {
                var result = _genreService.Add(model);
                if (result)
                {
                    TempData["msg"] = "Successfully Add New Genre";
                    return Json(new { success = true,Genre=model });
                }
                else
                {
                    TempData["msg"] = "Error Add New Genre";
                    return Json(new { success = false });
                }
			}
			catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success=false });
            }
        }
        public IActionResult Edit(int id)
        {
            var result = _genreService.GetById(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Update(Genre model)
        {
            if (ModelState.IsValid)
            {
                var result = _genreService.Update(model);
                if (result)
                {
                    TempData["msg"] = "Update Genre Successfully!";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
                else
                {
                    TempData["msg"] = "Error Update Genre";
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
            }
            else
            {
                return RedirectToAction(nameof(Edit));
            }
        }
        public ActionResult Delete(int id)
        {
            var result = _genreService.Delete(id);
            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}
