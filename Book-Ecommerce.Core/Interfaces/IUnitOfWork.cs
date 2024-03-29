using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Categories { get; }
        public ICoverTypeRepository CoverTypes { get; }
        public IProductRepository Products { get; }
        public ICompanyRepository Companies { get; }
        public IApplicationUserRepository AppicationUsers { get; }
        public IShoppingCartRepository ShoppingCarts { get; }
        public IOrderDetailRepository OrderDetails { get; }
        public IOrderHeaderRepository OrderHeaders { get; }

        public int Save();

        
    }
}
