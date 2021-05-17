using BulkyBook.Models;
using BulkyBook.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;

        public CategoryController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {

            Category category = new Category();
            if (id == null)
            {
                // this is for create
                return View(category);
            }
            // this is for edit
            category = _unityOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unityOfWork.Category.Add(category);
                }
                else
                {
                    _unityOfWork.Category.Update(category);
                }
                _unityOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unityOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unityOfWork.Category.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unityOfWork.Category.Remove(objFromDb);
            _unityOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
