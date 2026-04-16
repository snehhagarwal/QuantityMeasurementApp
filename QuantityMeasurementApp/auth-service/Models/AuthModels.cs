using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class RegisterDto
    {
        [Required][MinLength(3)][MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required][EmailAddress][MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required][MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class ValidateTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    public class TokenClaimsDto
    {
        public bool IsValid { get; set; }
        public long? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
