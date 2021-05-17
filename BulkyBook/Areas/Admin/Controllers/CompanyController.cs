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
    public class CompanyController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;

        public CompanyController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {

            Company company = new Company();
            if (id == null)
            {
                // this is for create
                return View(company);
            }
            // this is for edit
            company = _unityOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unityOfWork.Company.Add(company);
                }
                else
                {
                    _unityOfWork.Company.Update(company);
                }
                _unityOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unityOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unityOfWork.Company.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unityOfWork.Company.Remove(objFromDb);
            _unityOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
