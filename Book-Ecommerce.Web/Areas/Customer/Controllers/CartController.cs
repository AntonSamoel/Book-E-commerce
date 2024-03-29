using Book_Ecommerce.Core.Const;
using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Core.ViewModels;
using Book_Ecommerce.DataAcess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Book_Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var userId =  claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM shoppingCartVM = new ShoppingCartVM();
			shoppingCartVM.ShoppingCartList = await _unitOfWork.ShoppingCarts.FindAllAsync(sc => sc.ApplicationUserId == userId, includes: new string[] { "Product" });
            shoppingCartVM.OrderHeader = new();

			foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCart.Price = shoppingCart.Product.Price;
                shoppingCartVM.OrderHeader.OrderTotal += shoppingCart.Price * shoppingCart.Count;
            }
            return View(shoppingCartVM);
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cartDb = await _unitOfWork.ShoppingCarts.GetByIdAsync(cartId);

            await _unitOfWork.ShoppingCarts.Increment(cartDb,1);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartDb = await _unitOfWork.ShoppingCarts.GetByIdAsync(cartId);

            if(cartDb.Count<=1)
                 _unitOfWork.ShoppingCarts.Remove(cartDb);
            else
                await _unitOfWork.ShoppingCarts.Decrement(cartDb,1);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartDb = await _unitOfWork.ShoppingCarts.GetByIdAsync(cartId);

             _unitOfWork.ShoppingCarts.Remove(cartDb);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM shoppingCartVM = new ShoppingCartVM();
			shoppingCartVM.ShoppingCartList = await _unitOfWork.ShoppingCarts.FindAllAsync(sc => sc.ApplicationUserId == userId, includes: new string[] { "Product" });
			shoppingCartVM.OrderHeader = new();
			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.AppicationUsers.Find(au=>au.Id== userId);

			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
			//shoppingCartVM.OrderHeader.Email = shoppingCartVM.OrderHeader.ApplicationUser.Email;
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.Address = shoppingCartVM.OrderHeader.ApplicationUser.Address;

			foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
			{
				shoppingCart.Price = shoppingCart.Product.Price;
				shoppingCartVM.OrderHeader.OrderTotal += shoppingCart.Price * shoppingCart.Count;
			}

			return View(shoppingCartVM);
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(ShoppingCartVM shoppingCartVM)
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM.ShoppingCartList = await _unitOfWork.ShoppingCarts.FindAllAsync(sc => sc.ApplicationUserId == userId, includes: new string[] { "Product" });

            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            ApplicationUser applicationUser = _unitOfWork.AppicationUsers.Find(au=>au.Id==userId);

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
				shoppingCartVM.OrderHeader.PaymentStatus = PaymentStatus.Pending;
				shoppingCartVM.OrderHeader.OrderStatus = OrderStatus.Pending;
			}
            else
            {
				shoppingCartVM.OrderHeader.PaymentStatus = PaymentStatus.DelayedPayment;
				shoppingCartVM.OrderHeader.OrderStatus = OrderStatus.Approved;
			}

			foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
			{
				shoppingCart.Price = shoppingCart.Product.Price;
				shoppingCartVM.OrderHeader.OrderTotal += shoppingCart.Price * shoppingCart.Count;
			}

            _unitOfWork.OrderHeaders.Add(shoppingCartVM.OrderHeader);

            foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = shoppingCartVM.OrderHeader.Id,
                    ProductId = shoppingCart.ProductId,
                    Price = shoppingCart.Price,
                    Count = shoppingCart.Count
                };
                _unitOfWork.OrderDetails.Add(orderDetail);
            }

            // Stripe Settings 

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:44392/";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                      "card",
                    },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                };

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeaders.UpdateSessionAndPaymentId(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            else
            {
				return RedirectToAction("OrderConfirmation", "Cart", new { id = shoppingCartVM.OrderHeader.Id });
			}
		}

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaders.GetById(id);

            if(orderHeader.PaymentStatus!=PaymentStatus.DelayedPayment)
            {
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeaders.UpdateStatus(orderHeader.Id, OrderStatus.Approved, PaymentStatus.Approved);
                    _unitOfWork.OrderHeaders.UpdateSessionAndPaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
					List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCarts.FindAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
					_unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);
					_unitOfWork.Save();
					return View(id);
				}
                else
                {
                    return RedirectToAction(nameof(Index));
                }
			}
            else
            {
                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCarts.FindAll(s => s.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);
                _unitOfWork.Save();
                return View(id);
			}


		}

    }
}
