using Application.Response;
using Application.Services.Auth.Dtos;
using Application.Services.Otp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> SignUpAsync(RegisterRequestDto model);
        Task<ApiResponse<string>> LoginAsync(LoginRequestDto model);
        Task<ApiResponse<AuthResponseDto>> VerifyOtpAndGetTokenAsync(VerifyOtpRequestDto model);
        Task<ApiResponse<string>> CompleteProfileAsync(ProfileDto model, Guid Id);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
    }
}
