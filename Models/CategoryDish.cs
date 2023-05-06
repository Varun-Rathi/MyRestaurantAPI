using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstRestaurantAPI.Models
{
    public class CategoryDish
    {

            public int CategoryId { get; set; }
            public int DishId { get; set; }

            public virtual Category? Category { get; set; }
            public virtual Dish? Dish { get; set; }
            public bool IsDeleted { get; set; }

    }
}
