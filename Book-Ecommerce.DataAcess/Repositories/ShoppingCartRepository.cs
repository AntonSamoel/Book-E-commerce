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
    public class ShoppingCartRepository : MainRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> Decrement(ShoppingCart item, int count)
        {
            item.Count -= count;
           await _context.SaveChangesAsync();
            return item.Count;
        }

        public async Task<int> Increment(ShoppingCart item, int count)
        {
            item.Count += count;
            await _context.SaveChangesAsync(); 
            return item.Count;
        }
    }
    
}
