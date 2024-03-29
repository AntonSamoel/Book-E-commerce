using Book_Ecommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Core.Interfaces
{
    public interface IShoppingCartRepository : IMainRepository<ShoppingCart>
    {
        public Task<int> Increment(ShoppingCart item, int count);
        public Task<int> Decrement(ShoppingCart item, int count);
    }
}
