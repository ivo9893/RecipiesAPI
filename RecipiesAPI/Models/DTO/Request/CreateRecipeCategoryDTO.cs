
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models.DTO.Request
{
    public class CreateRecipeCategoryDTO
    {
        [Required(ErrorMessage = "CategoryId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive number.")]
        public int CategoryId { get; set; }
    }
}
