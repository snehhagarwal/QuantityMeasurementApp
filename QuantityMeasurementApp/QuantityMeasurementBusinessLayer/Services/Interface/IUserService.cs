using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Services.Interface
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> LoginAsync(LoginDto request, CancellationToken cancellationToken = default);
    }
}
