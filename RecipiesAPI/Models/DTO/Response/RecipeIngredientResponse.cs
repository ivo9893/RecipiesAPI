namespace RecipiesAPI.Models.DTO.Responce
{
    public class RecipeIngredientResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}
