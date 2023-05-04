using Microsoft.EntityFrameworkCore;

namespace CodeFirstRestaurantAPI.Models
{
    public class RestaurantAppContext: DbContext
    {
        public RestaurantAppContext(DbContextOptions<RestaurantAppContext> options): base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<MenuCategory>()
           .HasKey(mc => new { mc.MenuId, mc.CategoryId });


            modelBuilder.Entity<CategoryDish>()
             .HasKey(cd => new { cd.CategoryId, cd.DishId });


            modelBuilder.Entity<MenuCategory>()
                .HasOne(mc => mc.Menu)
                .WithMany(m => m.MenuCategories)
                .HasForeignKey(mc => mc.MenuId);

            modelBuilder.Entity<MenuCategory>()
                .HasOne(mc => mc.Category)
                .WithMany(c => c.MenuCategories)
                .HasForeignKey(mc => mc.CategoryId);

           

            modelBuilder.Entity<CategoryDish>()
                .HasOne(cd => cd.Category)
                .WithMany(c => c.CategoryDishes)
                .HasForeignKey(cd => cd.CategoryId);

            modelBuilder.Entity<CategoryDish>()
                .HasOne(cd => cd.Dish)
                .WithMany(d => d.CategoryDishes)
                .HasForeignKey(cd => cd.DishId);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder); 
        //}

        public DbSet<CategoryDish> CategoryDishes { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
