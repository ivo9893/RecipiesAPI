using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)] 
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public DateTime? RevokedAt { get; set; } 

        public bool IsRevoked => RevokedAt != null; 

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } 

    }
}
