using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _unitOfWork.Categories.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category categoy)
        {
            if (!ModelState.IsValid)
                return View(categoy);

            await _unitOfWork.Categories.AddAsync(categoy);
            TempData["sucess"] = "Category created successfully";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var category = await _unitOfWork.Categories.GetByIdAsync(id ?? 0);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category categoy)
        {
            if (!ModelState.IsValid)
                return View(categoy);

            _unitOfWork.Categories.Update(categoy);
            TempData["sucess"] = "Category updated successfully";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = await _unitOfWork.Categories.GetByIdAsync(id ?? 0);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(Category category)
        {
            if (category is null)
                return NotFound();

            _unitOfWork.Categories.Remove(category);
            TempData["sucess"] = "Category deleted successfully";


            return RedirectToAction("index");
        }

    }
}
