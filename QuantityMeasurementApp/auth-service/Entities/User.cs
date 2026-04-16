using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required][MaxLength(100)][Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required][MaxLength(200)][Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required][Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(50)][Column("role")]
        public string Role { get; set; } = "User";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)][Column("google_id")]
        public string? GoogleId { get; set; }

        [MaxLength(200)][Column("full_name")]
        public string? FullName { get; set; }

        [MaxLength(1000)][Column("picture_url")]
        public string? PictureUrl { get; set; }
    }
}
