using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// Request body for POST /api/v1/auth/register
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Plain-text password supplied by the user.
        /// Will be hashed with BCrypt before storage — never stored as-is.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for POST /api/v1/auth/login
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response body returned on successful register or login.
    /// Contains the JWT token the client must send in subsequent requests.
    ///
    /// Usage: add to every protected request header:
    ///   Authorization: Bearer {token}
    /// </summary>
    public class AuthResponseDto
    {
        public string Token     { get; set; } = string.Empty;
        public string Username  { get; set; } = string.Empty;
        public string Email     { get; set; } = string.Empty;
        public string Role      { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
