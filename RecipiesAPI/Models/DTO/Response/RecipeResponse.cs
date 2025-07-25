namespace RecipiesAPI.Models.DTO.Responce
{
    public class RecipeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan CookTime { get; set; }
        public TimeSpan PrepTime { get; set; }
        public int Servings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Instructions { get; set; }

        // Nested DTO for Author to avoid cycles
        public AuthorResponse Author { get; set; }

        // Flatten or map related collections as needed
        public List<RecipeIngredientResponse> Ingredients { get; set; }
        public List<ImageResponse> Images { get; set; }
        public List<RecipeCategoryResponse> Categories { get; set; }
    }
}
