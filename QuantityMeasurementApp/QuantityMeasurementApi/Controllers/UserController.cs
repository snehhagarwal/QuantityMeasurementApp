using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Dto.Auth;
using QuantityMeasurementBusinessLayer.Services.Interface;

namespace QuantityMeasurementApi.Controllers
{
    /// <summary>User signup, login, and JWT issuance.</summary>
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;

        public UserController(IUserService users) => _users = users;

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
    }
}
