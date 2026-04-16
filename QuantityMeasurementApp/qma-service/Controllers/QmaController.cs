using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QmaService.Interfaces;
using QmaService.Models;

namespace QmaService.Controllers;

[ApiController]
[Route("api/v1/quantities")]
[Produces("application/json")]
public class QuantityMeasurementController(IQmaService service) : ControllerBase
{
    private long? UserId()
    {
        var c = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return c != null && long.TryParse(c.Value, out var id) ? id : null;
    }

    // ── Public calculation endpoints ──────────────────────────────────────

    [AllowAnonymous]
    [HttpPost("compare")]
    public async Task<IActionResult> Compare([FromBody] QuantityInputDto input, CancellationToken ct)
        => Ok(await service.CompareAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, UserId(), ct));

    [AllowAnonymous]
    [HttpPost("convert")]
    public async Task<IActionResult> Convert([FromBody] QuantityInputDto input, CancellationToken ct)
        => Ok(await service.ConvertAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, UserId(), ct));

    [AllowAnonymous]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] QuantityInputDto input, CancellationToken ct)
        => Ok(await service.AddAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, UserId(), ct));

    [AllowAnonymous]
    [HttpPost("subtract")]
    public async Task<IActionResult> Subtract([FromBody] QuantityInputDto input, CancellationToken ct)
        => Ok(await service.SubtractAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, UserId(), ct));

    [AllowAnonymous]
    [HttpPost("divide")]
    public async Task<IActionResult> Divide([FromBody] QuantityInputDto input, CancellationToken ct)
        => Ok(await service.DivideAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, UserId(), ct));

    // ── Authenticated history endpoints ───────────────────────────────────

    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized(new { message = "User ID not found in token." });
        return Ok(await service.GetHistoryByUserAsync(userId.Value, ct));
    }

    [Authorize]
    [HttpGet("history/operation/{operationType}")]
    public async Task<IActionResult> GetHistoryByOperation(string operationType, CancellationToken ct)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized(new { message = "User ID not found in token." });
        return Ok(await service.GetHistoryByOperationTypeAndUserAsync(operationType, userId.Value, ct));
    }

    [Authorize]
    [HttpGet("history/type/{measurementType}")]
    public async Task<IActionResult> GetHistoryByType(string measurementType, CancellationToken ct)
        => Ok(await service.GetHistoryByMeasurementTypeAsync(measurementType, ct));

    [Authorize]
    [HttpGet("history/errored")]
    public async Task<IActionResult> GetErrorHistory(CancellationToken ct)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized(new { message = "User ID not found in token." });
        return Ok(await service.GetErrorHistoryByUserAsync(userId.Value, ct));
    }

    [Authorize]
    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory(CancellationToken ct)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized(new { message = "User ID not found in token." });
        var deleted = await service.ClearHistoryByUserAsync(userId.Value, ct);
        return Ok(new { message = $"Deleted {deleted} records.", deleted });
    }

    [Authorize]
    [HttpGet("count/{operationType}")]
    public async Task<IActionResult> GetCount(string operationType, CancellationToken ct)
        => Ok(await service.CountByOperationTypeAsync(operationType, ct));
}
