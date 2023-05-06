using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstRestaurantAPI.Models
{
    public class Category
    {
//        •	CategoryID(primary key)
//•	CategoryName
//•	CategoryDescription
//•	CategoryImage

        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string?  CategoryName { get; set; }

        public string? CategoryDescription { get; set; }

        public string? CategoryImage { get; set; }

        [NotMapped]
        public IFormFile? CategoryImageFile { get; set; }
        public bool IsDeleted { get; set; }


        public virtual ICollection<CategoryDish>? CategoryDishes { get; set; }
        public virtual ICollection<MenuCategory>? MenuCategories { get; set; }
    }
}
