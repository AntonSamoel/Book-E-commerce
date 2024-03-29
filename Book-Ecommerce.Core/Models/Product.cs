﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        [Required]
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Required]
        [Display(Name ="Cover Type")]
        public int CoverTypeId { get; set; }
        public CoverType? CoverType { get; set; }
    }
}
