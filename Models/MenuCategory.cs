using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstRestaurantAPI.Models
{

    public class MenuCategory
    {

        public int MenuId { get; set; }
        public int CategoryId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Menu? Menu { get; set; }
        public virtual Category? Category { get; set; }

        public int DisplayOrder { get; set; }
    }
}
