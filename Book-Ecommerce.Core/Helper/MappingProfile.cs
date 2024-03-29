using AutoMapper;
using Book_Ecommerce.Core.Models;
using Book_Ecommerce.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Core.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductVM>();
            CreateMap<ProductVM, Product>();
        }
    }
}
