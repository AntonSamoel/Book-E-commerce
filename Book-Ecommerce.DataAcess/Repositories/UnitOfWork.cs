using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.DataAcess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; private set; }

        public ICoverTypeRepository CoverTypes { get; private set; }

        public IProductRepository Products { get; private set; }
        public ICompanyRepository Companies { get; private set; }

        public IApplicationUserRepository AppicationUsers { get; private set; }

        public IShoppingCartRepository ShoppingCarts { get; private set; }

		public IOrderDetailRepository OrderDetails { get; private set; }

		public IOrderHeaderRepository OrderHeaders { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(context);
            CoverTypes = new CoverTypeRepository(context);
            Products = new ProductRepository(context);
            Companies = new CompanyRepository(context);
            AppicationUsers = new ApplicationUserRepository(context);
            ShoppingCarts = new ShoppingCartRepository(context);
			OrderDetails = new OrderDetailRepository(context);
            OrderHeaders = new OrderHeaderRepository(context);

		}

		public int Save()
		{
			return _context.SaveChanges();
		}
	}
}
