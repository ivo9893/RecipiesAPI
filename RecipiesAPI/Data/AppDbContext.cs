using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Models;

namespace RecipiesAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }

        // DbSet properties for each of your entities (tables)
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints if not handled by conventions

            // User and Recipe (One-to-Many: User can author many Recipes)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // Or .Cascade, depending on your desired behavior

            // Recipe and RecipeIngredient (One-to-Many: Recipe has many Ingredients)
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade); // If recipe is deleted, ingredients should also be deleted

            // Recipe and Image (One-to-Many: Recipe has many Images)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Images)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade); // If recipe is deleted, images should also be deleted

            // Many-to-Many relationship between Recipe and Category using RecipeCategory
            modelBuilder.Entity<RecipeCategory>()
                .HasOne(rc => rc.Recipe)
                .WithMany(r => r.RecipeCategories)
                .HasForeignKey(rc => rc.RecipeId);

            modelBuilder.Entity<RecipeCategory>()
                .HasOne(rc => rc.Category)
                .WithMany(c => c.RecipeCategories)
                .HasForeignKey(rc => rc.CategoryId);

            // Optional: Define composite primary key for RecipeCategory if 'Id' isn't used
            // and you want RecipeId and CategoryId to be the primary key.
            // If RecipeCategory has its own 'Id' as shown in the schema, this is not needed for the PK.
            // However, you might want to add a unique constraint to prevent duplicate entries:
            modelBuilder.Entity<RecipeCategory>()
                .HasIndex(rc => new { rc.RecipeId, rc.CategoryId })
                .IsUnique();

            // Data Seeding (Optional, but good for initial data)
            // Example for Category:
            // modelBuilder.Entity<Category>().HasData(
            //     new Category { Id = 1, Name = "Salad" },
            //     new Category { Id = 2, Name = "Meat" },
            //     new Category { Id = 3, Name = "Dessert" }
            // );
        }
    }
}
