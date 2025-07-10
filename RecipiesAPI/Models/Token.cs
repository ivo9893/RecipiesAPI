using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipiesAPI.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)] // Or appropriate length for your token generation strategy
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Auto-set creation time

        public DateTime? RevokedAt { get; set; } // Nullable, set when revoked

        public bool IsRevoked => RevokedAt != null; // Helper property

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property to the User

        // Optional: Can add more metadata like IP address, client ID etc.
        // [StringLength(45)]
        // public string IpAddress { get; set; }
    }
}
