using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.DataAcess.Repositories
{
	public class OrderHeaderRepository : MainRepository<OrderHeader>, IOrderHeaderRepository
	{
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
		}

		public bool UpdateSessionAndPaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = GetById(id);
			if (orderFromDb != null)
			{
				orderFromDb.PaymentDate = DateTime.Now;
				orderFromDb.SessionId = sessionId;
				orderFromDb.PaymentIntentId = paymentIntentId;
				return true;
			}
			return false;
		}

		public bool UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = GetById(id);
			if(orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if(paymentStatus != null)
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
				return true;
			}
			return false;
		}
	}
}
