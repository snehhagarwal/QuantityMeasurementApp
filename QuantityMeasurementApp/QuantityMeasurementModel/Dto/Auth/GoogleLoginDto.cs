using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto.Auth
{
    /// <summary>Request body for POST /api/v1/users/google-login.</summary>
    public class GoogleLoginDto
    {
        [Required(ErrorMessage = "Google ID token is required.")]
        public string IdToken { get; set; } = string.Empty;
    }
}