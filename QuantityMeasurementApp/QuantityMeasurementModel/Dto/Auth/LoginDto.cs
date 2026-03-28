using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto.Auth
{
    /// <summary>
    /// UC17: Request body DTO for POST /api/v1/users/login.
    /// Username and password are the only fields needed for authentication.
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
