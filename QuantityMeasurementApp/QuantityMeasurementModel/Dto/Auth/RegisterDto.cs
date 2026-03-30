using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto.Auth
{
    /// <summary>
    /// UC17: Request body DTO for POST /api/v1/users/signup.
    /// Contains user registration details — username, email, password.
    /// Password is plain text here; it is BCrypt-hashed before storage and
    /// never persisted as plain text anywhere.
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3,   ErrorMessage = "Username must be at least 3 characters.")]
        [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Plain-text password supplied by the user.
        /// BCrypt.HashPassword() is called in UserService before storage.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
