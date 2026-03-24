using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementBusinessLayer.Services.Interface;

namespace QuantityMeasurementApi.Controllers
{
    /// <summary>Quantity operations — compare, convert, add, subtract, divide and history.</summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/quantities")]
    [Produces("application/json")]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
            => _service = service;

        [HttpPost("compare")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PerformCompare([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.CompareAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken)
                .ConfigureAwait(false));
                

        [HttpPost("convert")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PerformConvert([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.ConvertAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken)
                .ConfigureAwait(false));

        [HttpPost("add")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PerformAdd([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.AddAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken)
                .ConfigureAwait(false));

        [HttpPost("subtract")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PerformSubtract([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.SubtractAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken)
                .ConfigureAwait(false));

        [HttpPost("divide")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PerformDivide([FromBody] QuantityInputDto input, CancellationToken cancellationToken)
            => Ok(await _service.DivideAsync(input.ThisQuantityDTO, input.ThatQuantityDTO, cancellationToken)
                .ConfigureAwait(false));

        [HttpGet("history")]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
            => Ok(await _service.GetAllHistoryAsync(cancellationToken).ConfigureAwait(false));

        [HttpGet("history/operation/{operationType}")]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryByOperationType(string operationType, CancellationToken cancellationToken)
            => Ok(await _service.GetHistoryByOperationTypeAsync(operationType, cancellationToken).ConfigureAwait(false));

        [HttpGet("history/type/{measurementType}")]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryByMeasurementType(string measurementType, CancellationToken cancellationToken)
            => Ok(await _service.GetHistoryByMeasurementTypeAsync(measurementType, cancellationToken).ConfigureAwait(false));

        [HttpGet("history/errored")]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetErrorHistory(CancellationToken cancellationToken)
            => Ok(await _service.GetErrorHistoryAsync(cancellationToken).ConfigureAwait(false));

        [HttpGet("count/{operationType}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount(string operationType, CancellationToken cancellationToken)
            => Ok(await _service.CountByOperationTypeAsync(operationType, cancellationToken).ConfigureAwait(false));
    }
}
