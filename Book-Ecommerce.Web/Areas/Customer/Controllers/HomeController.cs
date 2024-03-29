using Book_Ecommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Book_Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products  = _unitOfWork.Products.GetAll();
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            var includes = new string[] { "Category","CoverType" };
            var product  = _unitOfWork.Products.Find(p=>p.Id== productId, includes);
            ShoppingCart shoppingCart = new() { Product = product };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart  shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var shoppingCartDb = _unitOfWork.ShoppingCarts.Find(sc=>sc.ProductId==shoppingCart.ProductId
                && sc.ApplicationUserId==shoppingCart.ApplicationUserId);

            if(shoppingCartDb == null)
            {
                _unitOfWork.ShoppingCarts.Add(shoppingCart);
            }
            else
            {
               await _unitOfWork.ShoppingCarts.Increment(shoppingCartDb, shoppingCart.Count);
                
            }

            return RedirectToAction(nameof(Index));
        }




        [HttpPost]
        public IActionResult Search(string data)
        {
            var products  = _unitOfWork.Products.FindAll(p=>p.Title.ToLower().Contains(data.ToLower()) || p.Description.ToLower().Contains(data.ToLower()));
            return View("index",products);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}