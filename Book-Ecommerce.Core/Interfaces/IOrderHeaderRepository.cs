using Book_Ecommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Core.Interfaces
{
    public interface IOrderHeaderRepository : IMainRepository<OrderHeader>
    {
        public bool UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        public bool UpdateSessionAndPaymentId(int id, string sessionId, string paymentIntentId);
    }
}
