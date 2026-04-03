using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Dto.Auth;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementBusinessLayer.Services.Interface;
using Google.Apis.Auth;

namespace QuantityMeasurementBusinessLayer.Services.Implementation
{
    /// <summary>Signup / login; BCrypt hash + per-user salt, optional AES envelope, JWT issuance.</summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenService _jwt;
        private readonly IAesEncryptionService _crypto;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _config;  // ← ADDED

        public UserService(
            IUserRepository users,
            IJwtTokenService jwt,
            IAesEncryptionService crypto,
            ILogger<UserService> logger,
            IConfiguration config)  // ← ADDED
        {
            _users = users;
            _jwt = jwt;
            _crypto = crypto;
            _logger = logger;
            _config = config;  // ← ADDED
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Register attempt for {Username}", request.Username);

            if (await _users.ExistsByUsernameAsync(request.Username, cancellationToken))
            {
                _logger.LogWarning("Username taken: {Username}", request.Username);
                throw new ArgumentException($"Username '{request.Username}' is already taken.");
            }

            if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                _logger.LogWarning("Email registered: {Email}", request.Email);
                throw new ArgumentException($"Email '{request.Email}' is already registered.");
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor: 12);
            string saltedHash = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);
            string encryptedHash = _crypto.Encrypt(saltedHash);

            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = encryptedHash,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _users.AddAsync(user, cancellationToken);
            _logger.LogInformation("User registered: {Username} id={Id}", user.Username, user.Id);

            return _jwt.CreateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Login attempt for {Username}", request.Username);

            var user = await _users.GetByUsernameAsync(request.Username, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("User not found: {Username}", request.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            string saltedHash;
            try
            {
                saltedHash = _crypto.Decrypt(user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decrypt failed for {Username}", request.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, saltedHash))
            {
                _logger.LogWarning("Bad password for {Username}", request.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            _logger.LogInformation("Login ok for {Username}", request.Username);
            return _jwt.CreateToken(user);
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(string googleIdToken, CancellationToken ct = default)
        {
            // 1. Validate the Google ID token
            var clientId = _config["Google:ClientId"]
                ?? throw new InvalidOperationException("Google:ClientId is missing.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning("Invalid Google token: {Msg}", ex.Message);
                throw new UnauthorizedAccessException("Invalid or expired Google token.");
            }

            // 2. Find or create user
            var googleId = payload.Subject;
            var email = payload.Email.ToLowerInvariant();

            var user = await _users.GetByGoogleIdAsync(googleId, ct)
                    ?? await _users.GetByEmailAsync(email, ct);

            if (user == null)
            {
                // New user — auto-register
                var username = email.Split('@')[0].Replace(".", "_") + "_" + googleId[..6];
                user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = "GOOGLE_AUTH_NO_PASSWORD",
                    GoogleId = googleId,
                    FullName = payload.Name,
                    PictureUrl = payload.Picture,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _users.AddAsync(user, ct);
                _logger.LogInformation("New Google user created: {Email}", email);
            }
            else
            {
                // Existing user — refresh Google fields
                user.GoogleId = googleId;
                user.FullName = payload.Name;
                user.PictureUrl = payload.Picture;
                user.UpdatedAt = DateTime.UtcNow;
                await _users.UpdateAsync(user, ct);
            }

            return _jwt.CreateToken(user);
        }
    }
}