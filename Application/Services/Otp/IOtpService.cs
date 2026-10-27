using Application.Response;
using Application.Services.Otp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Otp
{
    public interface IOtpService
    {
        Task<ApiResponse<string>> SendOtpAsync(string phoneNumber);
        Task<ApiResponse<bool>> VerifyOtpAsync(VerifyOtpRequestDto dto);
    }
}
