using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstRestaurantAPI.Models
{
    public class Dish
    {
        [Key]
        public int DishId { get; set; }

        [Required]
        public string? DishName { get; set; }

        public string?  DishDescription { get; set;}
        
        public decimal DishPrice { get; set; }
        public string? DishImage { get; set; }
        [NotMapped]
        public IFormFile? DishImageFile { get; set; }

        public string? DishNature { get; set; }

        public virtual ICollection<CategoryDish>? CategoryDishes { get; set; }

    }
}
