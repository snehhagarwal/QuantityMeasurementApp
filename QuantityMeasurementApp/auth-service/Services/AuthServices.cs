using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AuthService.Data;
using AuthService.Entities;
using AuthService.Models;
using Google.Apis.Auth;

namespace AuthService.Services
{
    // ── AES Encryption ────────────────────────────────────────────────────────
    public class AesEncryptionService
    {
        private readonly byte[] _key;
        public AesEncryptionService(IConfiguration cfg)
            => _key = SHA256.HashData(Encoding.UTF8.GetBytes(
                cfg["Encryption:Key"] ?? throw new InvalidOperationException("Encryption:Key missing")));

        public string Encrypt(string plain)
        {
            using var aes = Aes.Create();
            aes.Key = _key; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();
            var cipher = aes.CreateEncryptor().TransformFinalBlock(Encoding.UTF8.GetBytes(plain), 0, plain.Length);
            var result = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string b64)
        {
            var combined = Convert.FromBase64String(b64);
            var iv = combined[..16];
            var cipher = combined[16..];
            using var aes = Aes.Create();
            aes.Key = _key; aes.IV = iv; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
            return Encoding.UTF8.GetString(aes.CreateDecryptor().TransformFinalBlock(cipher, 0, cipher.Length));
        }
    }

    // ── JWT Token Service ─────────────────────────────────────────────────────
    public class JwtTokenService
    {
        private readonly IConfiguration _cfg;
        public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

        public AuthResponseDto CreateToken(User user)
        {
            var secret = _cfg["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey missing");
            int hours = int.TryParse(_cfg["Jwt:ExpiryHours"], out int h) ? h : 24;
            var expiry = DateTime.UtcNow.AddHours(hours);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                notBefore: DateTime.UtcNow,
                expires: expiry,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiry
            };
        }

        public TokenClaimsDto ValidateToken(string token)
        {
            try
            {
                var secret = _cfg["Jwt:SecretKey"]!;
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = true, ValidIssuer = _cfg["Jwt:Issuer"],
                    ValidateAudience = true, ValidAudience = _cfg["Jwt:Audience"],
                    ValidateLifetime = true, ClockSkew = TimeSpan.Zero
                }, out _);

                var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                return new TokenClaimsDto
                {
                    IsValid = true,
                    UserId = long.TryParse(sub, out var id) ? id : null,
                    Username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value,
                    Email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value,
                    Role = principal.FindFirst(ClaimTypes.Role)?.Value
                };
            }
            catch
            {
                return new TokenClaimsDto { IsValid = false };
            }
        }
    }

    // ── User Service ──────────────────────────────────────────────────────────
    public class UserService
    {
        private readonly AuthDbContext _db;
        private readonly JwtTokenService _jwt;
        private readonly AesEncryptionService _crypto;
        private readonly IConfiguration _cfg;
        private readonly ILogger<UserService> _logger;

        public UserService(AuthDbContext db, JwtTokenService jwt, AesEncryptionService crypto,
            IConfiguration cfg, ILogger<UserService> logger)
        { _db = db; _jwt = jwt; _crypto = crypto; _cfg = cfg; _logger = logger; }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto req, CancellationToken ct)
        {
            if (await _db.Users.AnyAsync(u => u.Username == req.Username, ct))
                throw new ArgumentException($"Username '{req.Username}' is already taken.");
            if (await _db.Users.AnyAsync(u => u.Email == req.Email.ToLowerInvariant(), ct))
                throw new ArgumentException($"Email '{req.Email}' is already registered.");

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var user = new User
            {
                Username = req.Username.Trim(),
                Email = req.Email.Trim().ToLowerInvariant(),
                PasswordHash = _crypto.Encrypt(BCrypt.Net.BCrypt.HashPassword(req.Password, salt)),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Registered user {Username}", user.Username);
            return _jwt.CreateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto req, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username, ct)
                ?? throw new UnauthorizedAccessException("Invalid username or password.");
            if (!BCrypt.Net.BCrypt.Verify(req.Password, _crypto.Decrypt(user.PasswordHash)))
                throw new UnauthorizedAccessException("Invalid username or password.");
            return _jwt.CreateToken(user);
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(string idToken, CancellationToken ct)
        {
            var clientId = _cfg["Google:ClientId"] ?? throw new InvalidOperationException("Google:ClientId missing");
            GoogleJsonWebSignature.Payload payload;
            try { payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { clientId } }); }
            catch (InvalidJwtException ex) { throw new UnauthorizedAccessException("Invalid Google token: " + ex.Message); }

            var email = payload.Email.ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject || u.Email == email, ct);
            if (user == null)
            {
                user = new User
                {
                    Username = email.Split('@')[0].Replace(".", "_") + "_" + payload.Subject[..6],
                    Email = email, PasswordHash = "GOOGLE_AUTH_NO_PASSWORD",
                    GoogleId = payload.Subject, FullName = payload.Name, PictureUrl = payload.Picture,
                    Role = "User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
            }
            else
            {
                user.GoogleId = payload.Subject; user.FullName = payload.Name;
                user.PictureUrl = payload.Picture; user.UpdatedAt = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync(ct);
            return _jwt.CreateToken(user);
        }

        public async Task<User?> GetByIdAsync(long id, CancellationToken ct)
            => await _db.Users.FindAsync(new object[] { id }, ct);
    }
}
