using AutoMapper;
using RecipiesAPI.Models.DTO.Responce;
using RecipiesAPI.Models;
using RecipiesAPI.Models.DTO.Response;

namespace RecipiesAPI.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Recipe, RecipeResponse>()
           .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
           .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.RecipeCategories))
           .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
           .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

            CreateMap<User, AuthorResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<RecipeIngredient, RecipeIngredientResponse>();

            CreateMap<Image, ImageResponse>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<User, UserResponse>();

            CreateMap<RecipeCategory, RecipeCategoryResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Units, UnitsResponse>();
        }
    }
}
