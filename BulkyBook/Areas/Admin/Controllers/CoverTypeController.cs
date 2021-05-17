using BulkyBook.Models;
using BulkyBook.Repository.IRepository;
using BulkyBook.Utility;
using Dapper;
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
    public class CoverTypeController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;

        public CoverTypeController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Upsert(int? id)
        {

            CoverType coverType = new CoverType();
            if (id == null)
            {
                // this is for create
                return View(coverType);
            }
            // this is for edit
            var parameter = new DynamicParameters();
            parameter.Add(@"Id", id);
            coverType = _unityOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add(@"Name", coverType.Name);
                if (coverType.Id == 0)
                {
                    _unityOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                }
                else
                {
                    parameter.Add(@"Id", coverType.Id);
                    _unityOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                }
                    _unityOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unityOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add(@"Id", id);
            var objFromDb = _unityOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unityOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            _unityOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
