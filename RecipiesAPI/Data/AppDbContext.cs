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

        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
