using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthService.Models;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserService _users;
        public UserController(UserService users) => _users = users;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] RegisterDto req, CancellationToken ct)
        {
            try { return StatusCode(201, await _users.RegisterAsync(req, ct)); }
            catch (ArgumentException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto req, CancellationToken ct)
        {
            try { return Ok(await _users.LoginAsync(req, ct)); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto req, CancellationToken ct)
        {
            try { return Ok(await _users.GoogleLoginAsync(req.IdToken, ct)); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile(CancellationToken ct)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(idClaim, out var userId)) return Unauthorized();
            var user = await _users.GetByIdAsync(userId, ct);
            if (user == null) return NotFound();
            return Ok(new { user.Id, user.Username, user.Email, user.Role, user.CreatedAt, user.UpdatedAt });
        }
    }

    /// <summary>Internal endpoint — called by api-gateway to validate JWT tokens.</summary>
    [ApiController]
    [Route("internal/auth")]
    public class InternalAuthController : ControllerBase
    {
        private readonly JwtTokenService _jwt;
        public InternalAuthController(JwtTokenService jwt) => _jwt = jwt;

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] ValidateTokenDto req)
            => Ok(_jwt.ValidateToken(req.Token));
    }
}
