using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstRestaurantAPI.Models
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }
        [Required]
        public string? MenuName { get; set; }
        public string? MenuDescription { get; set; } 
        public string? MenuImagePath { get; set; }
        [NotMapped]
        public IFormFile? MenuImage { get; set; }
        public bool IsDeleted { get; set; }


        public virtual ICollection<MenuCategory>? MenuCategories { get; set; }
    }
}
