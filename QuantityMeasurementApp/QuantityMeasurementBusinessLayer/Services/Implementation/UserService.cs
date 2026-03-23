using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementBusinessLayer.Services.Interface;

namespace QuantityMeasurementBusinessLayer.Services.Implementation
{
    /// <summary>Signup / login; BCrypt hash + per-user salt, optional AES envelope, JWT issuance.</summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenService _jwt;
        private readonly IAesEncryptionService _crypto;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository users,
            IJwtTokenService jwt,
            IAesEncryptionService crypto,
            ILogger<UserService> logger)
        {
            _users = users;
            _jwt = jwt;
            _crypto = crypto;
            _logger = logger;
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
    }
}
