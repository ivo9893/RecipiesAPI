namespace RecipiesAPI.Models.DTO
{
    public class CreateRecipeIngredientDTO
    {
        public string Name { get; set; }
        public int RecipeId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}
