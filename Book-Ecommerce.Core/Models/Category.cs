using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Display Order")]
        [Range(1, 100,ErrorMessage ="The value must be between 1 and 100")]  
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
