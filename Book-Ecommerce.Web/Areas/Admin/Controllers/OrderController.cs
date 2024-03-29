using Book_Ecommerce.Core.Const;
using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace Book_Ecommerce.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
        public IUnitOfWork _unitOfWork { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}
        public IActionResult Index()
		{
			return View();
		}
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeaders.Find(oh=>oh.Id == orderId,new string[] { "ApplicationUser" }),
                OrderDetails = _unitOfWork.OrderDetails.FindAll(od=>od.OrderId== orderId,new string[] {"Product"})
            };
          
            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetail(OrderVM orderVM)
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeaders.GetByIdAsync(orderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.Address = orderVM.OrderHeader.Address;

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.Save();

            TempData["sucess"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Role_User_Comp)]
        public async Task<IActionResult> CompanyPayNow(OrderVM orderVM)
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeaders.FindAsync(od=> od.Id==orderVM.OrderHeader.Id,new string[] {"ApplicationUser"});
            var orderDetails = await _unitOfWork.OrderDetails.FindAllAsync(od=>od.OrderId == orderHeaderFromDb.Id, new string[] { "Product" });

            // Stripe Settings
            var domain = "https://localhost:44392/";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                    {
                      "card",
                    },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
            };

            foreach (var item in orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions()
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

            
            _unitOfWork.OrderHeaders.UpdateSessionAndPaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public async Task<IActionResult> PaymentConfirmation(int orderHeaderid)
        {
            var orderHeader = _unitOfWork.OrderHeaders.GetById(orderHeaderid);

            if (orderHeader.PaymentStatus == PaymentStatus.DelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaders.UpdateStatus(orderHeader.Id, orderHeader.OrderStatus, PaymentStatus.Approved);
                    _unitOfWork.OrderHeaders.UpdateSessionAndPaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
                else
                {
                    return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
                }

            }
            return View(orderHeaderid);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProcessing(OrderVM orderVM)
        {
            _unitOfWork.OrderHeaders.UpdateStatus(orderVM.OrderHeader.Id, OrderStatus.InProcess);
            _unitOfWork.Save();

            TempData["sucess"] = "Order Status Updated Successfully.";


            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipOrder(OrderVM orderVM)
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeaders.GetByIdAsync(orderVM.OrderHeader.Id);
            orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeaderFromDb.OrderStatus = OrderStatus.Shipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;

            if(orderVM.OrderHeader.PaymentStatus == PaymentStatus.DelayedPayment)
                orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);

            _unitOfWork.Save();

            TempData["sucess"] = "Order Status Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(OrderVM orderVM)
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeaders.GetByIdAsync(orderVM.OrderHeader.Id);

            if (orderHeaderFromDb.PaymentStatus == PaymentStatus.Approved)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId // this will refund the exact amount in the payment
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderFromDb.Id, OrderStatus.Cancelled, PaymentStatus.Refunded);
            }
            else
            {
                _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderFromDb.Id, OrderStatus.Cancelled, PaymentStatus.Cancelled);
            }

            _unitOfWork.Save();
            TempData["sucess"] = "Order Canceled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        #region API Calls
        [HttpGet]
		public IActionResult GetAll(string status)
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<OrderHeader> orderHeaders;

            if(User.IsInRole(Roles.Role_Admin) || User.IsInRole(Roles.Role_Employee))
                orderHeaders = _unitOfWork.OrderHeaders.FindAll(oh => oh.Id > 0, new string[] { "ApplicationUser" });
            else
                orderHeaders = _unitOfWork.OrderHeaders.FindAll(oh => oh.Id > 0 && oh.ApplicationUserId==userId, new string[] { "ApplicationUser" });

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == PaymentStatus.Pending);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == OrderStatus.InProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == OrderStatus.Shipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == OrderStatus.Approved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
