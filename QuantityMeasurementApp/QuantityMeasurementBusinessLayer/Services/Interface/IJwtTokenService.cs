using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Dto.Auth;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementBusinessLayer.Services.Interface
{
    public interface IJwtTokenService
    {
        AuthResponseDto CreateToken(User user);
    }
}
