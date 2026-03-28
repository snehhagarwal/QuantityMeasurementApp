namespace QuantityMeasurementModel.Dto.Auth
{
    /// <summary>
    /// UC17: Response body returned on successful register or login.
    /// Contains the signed JWT token that the client must send in subsequent requests.
    ///
    /// Usage — add to every protected request header:
    ///   Authorization: Bearer {Token}
    ///
    /// Token expires at ExpiresAt (UTC). After expiry the client must log in again.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>Signed JWT token string — send as Bearer token in Authorization header.</summary>
        public string   Token     { get; set; } = string.Empty;

        /// <summary>The authenticated user's username.</summary>
        public string   Username  { get; set; } = string.Empty;

        /// <summary>The authenticated user's email address.</summary>
        public string   Email     { get; set; } = string.Empty;

        /// <summary>Role assigned to the user (e.g. "User", "Admin").</summary>
        public string   Role      { get; set; } = string.Empty;

        /// <summary>UTC datetime when the token expires.</summary>
        public DateTime ExpiresAt { get; set; }
    }
}
