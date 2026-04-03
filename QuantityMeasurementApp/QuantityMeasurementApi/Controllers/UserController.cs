using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Dto.Auth;
using QuantityMeasurementBusinessLayer.Services.Interface;
using QuantityMeasurementRepository.Interface;
using System.Security.Claims;

namespace QuantityMeasurementApi.Controllers
{
    /// <summary>User signup, login, JWT issuance, and profile retrieval.</summary>
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly IUserRepository _userRepo;

        public UserController(IUserService users, IUserRepository userRepo)
        {
            _users = users;
            _userRepo = userRepo;
        }

        /// <summary>Register a new account; returns JWT.</summary>
        [HttpPost("signup")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public Task<IActionResult> Signup([FromBody] RegisterDto request, CancellationToken cancellationToken)
            => RegisterInternalAsync(request, cancellationToken);

        private async Task<IActionResult> RegisterInternalAsync(RegisterDto request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _users.RegisterAsync(request, cancellationToken).ConfigureAwait(false);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Authenticate and receive JWT.</summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _users.LoginAsync(request, cancellationToken).ConfigureAwait(false);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>Get the profile of the currently authenticated user.</summary>
        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("sub")
                       ?? User.FindFirst("id");
            if (idClaim == null || !long.TryParse(idClaim.Value, out var userId))
                return Unauthorized(new { message = "User ID not found in token." });

            var user = await _userRepo.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user == null) return NotFound(new { message = "User not found." });

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                role = user.Role,
                createdAt = user.CreatedAt,
                updatedAt = user.UpdatedAt
            });
        }

        /// <summary>Sign in or register via Google ID token.</summary>
        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request, CancellationToken ct)
        {
            try
            {
                var response = await _users.GoogleLoginAsync(request.IdToken, ct);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}