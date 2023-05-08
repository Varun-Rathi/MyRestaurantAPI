using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstRestaurantAPI.Models
{
    public class Category
    {


        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string?  CategoryName { get; set; }

        public string? CategoryDescription { get; set; }

        public string? CategoryImage { get; set; }


        // IFormFile type indicates that this property can hold a file uploaded through a form. 
        //[NotMapped] attribute which means that this property is not going to be mapped to a column in the database table that is associated with this class.
        [NotMapped]
        public IFormFile? CategoryImageFile { get; set; }
        public bool IsDeleted { get; set; }

        // virtual indicates that this property can be overriden in derived classes

        //  this property can hold zero or more CategoryDish objects and can be overridden in derived classes.
        public virtual ICollection<CategoryDish>? CategoryDishes { get; set; }
        public virtual ICollection<MenuCategory>? MenuCategories { get; set; }
    }
}
