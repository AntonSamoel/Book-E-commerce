using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CoverType> coverTypes = await _unitOfWork.CoverTypes.GetAllAsync();
            return View(coverTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoverType coverType)
        {
            if (!ModelState.IsValid)
                return View(coverType);

            await _unitOfWork.CoverTypes.AddAsync(coverType);
            TempData["sucess"] = "Cover Type created successfully";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var coverType = await _unitOfWork.CoverTypes.GetByIdAsync(id ?? 0);
            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)
        {
            if (!ModelState.IsValid)
                return View(coverType);

            _unitOfWork.CoverTypes.Update(coverType);
            TempData["sucess"] = "Cover Type updated successfully";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverType = await _unitOfWork.CoverTypes.GetByIdAsync(id ?? 0);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        [HttpPost]
        public IActionResult Delete(CoverType coverType)
        {
            if (coverType is null)
                return NotFound();

            _unitOfWork.CoverTypes.Remove(coverType);
            TempData["sucess"] = "Cover Type deleted successfully";

            return RedirectToAction("index");
        }

    }
}
