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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new();

            productVM.CategoryList = _unitOfWork.Categories.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            productVM.CoverTypeList = _unitOfWork.CoverTypes.GetAll().Select(ct => new SelectListItem
            {
                Text = ct.Name,
                Value = ct.Id.ToString(),
            });

            if (id == 0 || id is null)
            {
                // Create new product
                return View(productVM);
            }
            else
            {
                // Update existing product
                var product = _unitOfWork.Products.GetById(id ?? 0);
                productVM = _mapper.Map<ProductVM>(product);
                productVM.CategoryList = _unitOfWork.Categories.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

                productVM.CoverTypeList = _unitOfWork.CoverTypes.GetAll().Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString(),
                });
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
				if (file is not null)
				{
					string fileName = Guid.NewGuid().ToString();
                    string path = Path.Combine(wwwRootPath, @"Images/Products");
                    var extension = Path.GetExtension(file.FileName);

                    if(productVM.ImageUrl != null)
                    {
                        var oldImagPath = Path.Combine(wwwRootPath, productVM.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagPath))
                            System.IO.File.Delete(oldImagPath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(path, fileName + extension),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.ImageUrl = @"/Images/Products/" + fileName + extension;
			    }

                var product = _mapper.Map<Product>(productVM);
                if (productVM.Id == 0)
                {
                    _unitOfWork.Products.Add(product);
                     TempData["sucess"] = "Product Added Successfully";

                }
                else
                {
                    _unitOfWork.Products.Update(product);
                    TempData["sucess"] = "Product Updated Successfully";
                }

                return RedirectToAction("Index");
			}
          
            return View(productVM);
        }

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            string[] includes = { "Category" };
            var productList = _unitOfWork.Products.FindAll(p => p.Id != 0, includes);
			return Json(new { data = productList });
		}
        [HttpDelete]
        public IActionResult Delete (int? id)
        {
            var product = _unitOfWork.Products.GetById(id??0); 
            if(product is null)
            {
                return Json(new{ success=false,message="Error While Deleting"});
            }

            var oldImagPath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldImagPath))
                System.IO.File.Delete(oldImagPath);

            _unitOfWork.Products.Remove(product);
            return Json(new { success = true, message = "Product Deleted Successfully" });
        }

        #endregion

    }


}
