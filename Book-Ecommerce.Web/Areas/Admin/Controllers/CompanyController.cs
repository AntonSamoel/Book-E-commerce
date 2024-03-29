using AutoMapper;
using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Book_Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if (id == 0 || id is null)
            {
                // Create new product
                return View(company);
            }
            else
            {
                // Update existing product
                company = _unitOfWork.Companies.GetById(id ?? 0);  
                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Companies.Add(company);
                     TempData["sucess"] = "Company Added Successfully";

                }
                else
                {
                    _unitOfWork.Companies.Update(company);
                    TempData["sucess"] = "Company Updated Successfully";
                }

                return RedirectToAction("Index");
			}
          
            return View(company);
        }

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            var companies = _unitOfWork.Companies.GetAll();
			return Json(new { data = companies });
		}
        [HttpDelete]
        public IActionResult Delete (int? id)
        {
            var company = _unitOfWork.Companies.GetById(id??0); 
            if(company is null)
            {
                return Json(new{ success=false,message="Error While Deleting"});
            }

            _unitOfWork.Companies.Remove(company);
            return Json(new { success = true, message = "Company Deleted Successfully" });
        }

        #endregion

    }


}
