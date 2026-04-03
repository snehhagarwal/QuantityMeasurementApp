using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementBusinessLayer.Services.Interface;
using System.Security.Claims;

namespace QuantityMeasurementApi.Controllers
{
    [ApiController]
    [Route("api/v1/quantities")]
    [Produces("application/json")]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
            => _service = service;

        // ── Helper: get userId from JWT claims ────────────────────────────────
        private long? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                     ?? User.FindFirst("sub")
                     ?? User.FindFirst("id")
                     ?? User.FindFirst("userId");
            return claim != null && long.TryParse(claim.Value, out var id) ? id : null;
        }

        // ── Calculation endpoints — PUBLIC ────────────────────────────────────

        [AllowAnonymous]
        [HttpPost("compare")]
        public async Task<IActionResult> PerformCompare([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.CompareAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken).ConfigureAwait(false));

        [AllowAnonymous]
        [HttpPost("convert")]
        public async Task<IActionResult> PerformConvert([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.ConvertAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken).ConfigureAwait(false));

        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<IActionResult> PerformAdd([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.AddAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken).ConfigureAwait(false));

        [AllowAnonymous]
        [HttpPost("subtract")]
        public async Task<IActionResult> PerformSubtract([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.SubtractAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken).ConfigureAwait(false));

        [AllowAnonymous]
        [HttpPost("divide")]
        public async Task<IActionResult> PerformDivide([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.DivideAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken).ConfigureAwait(false));

        // ── History endpoints — REQUIRES JWT ─────────────────────────────────

        /// <summary>Get history for the currently logged-in user only.</summary>
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User ID not found in token." });

            var result = await _service.GetHistoryByUserAsync(userId.Value, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>Get history filtered by operation type for the current user.</summary>
        [Authorize]
        [HttpGet("history/operation/{operationType}")]
        public async Task<IActionResult> GetHistoryByOperationType(string operationType, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User ID not found in token." });

            var result = await _service.GetHistoryByOperationTypeAndUserAsync(operationType, userId.Value, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>Get history filtered by measurement type for the current user.</summary>
        [Authorize]
        [HttpGet("history/type/{measurementType}")]
        public async Task<IActionResult> GetHistoryByMeasurementType(string measurementType, CancellationToken cancellationToken)
            => Ok(await _service.GetHistoryByMeasurementTypeAsync(measurementType, cancellationToken).ConfigureAwait(false));

        /// <summary>Get error history for the current user only.</summary>
        [Authorize]
        [HttpGet("history/errored")]
        public async Task<IActionResult> GetErrorHistory(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User ID not found in token." });

            var result = await _service.GetErrorHistoryByUserAsync(userId.Value, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>Clear ALL history for the current user.</summary>
        [Authorize]
        [HttpDelete("history")]
        public async Task<IActionResult> ClearHistory(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized(new { message = "User ID not found in token." });

            var deleted = await _service.ClearHistoryByUserAsync(userId.Value, cancellationToken).ConfigureAwait(false);
            return Ok(new { message = $"Deleted {deleted} records.", deleted });
        }

        [Authorize]
        [HttpGet("count/{operationType}")]
        public async Task<IActionResult> GetCount(string operationType, CancellationToken cancellationToken)
            => Ok(await _service.CountByOperationTypeAsync(operationType, cancellationToken).ConfigureAwait(false));
    }
}
