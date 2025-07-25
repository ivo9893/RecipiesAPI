using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Data;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Request;
using RecipiesAPI.Models.DTO.Responce;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.Equals(categoryDTO.Name));

            if (existingCategory != null)
            {
                throw new InvalidOperationException($"Category '{categoryDTO.Name}' already exists.");
            }

            var category = new Category
            {
                Name = categoryDTO.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<CategoryResponse>(category);

            return response;
        }
        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .ToListAsync();


            var response = _mapper.Map<List<CategoryResponse>>(categories);

            return response;
        }

    }
}
