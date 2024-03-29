using Book_Ecommerce.Core.Interfaces;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.DataAcess.Repositories
{
    public class CoverTypeRepository : MainRepository<CoverType>, ICoverTypeRepository
    {
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
        }


    }
}
