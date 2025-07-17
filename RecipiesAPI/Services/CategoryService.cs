using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.Equals(categoryDTO.Name));

            if(existingCategory != null)
            {
                throw new InvalidOperationException($"Category '{categoryDTO.Name}' already exists.");
            }

            var category = new Category
            {
                Name = categoryDTO.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

    }
}
