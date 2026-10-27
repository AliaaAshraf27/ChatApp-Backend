using Application.Services.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Cache
{
    public interface ICacheService
    {
        // OTP 
        Task SetOtpAsync(string phoneNumber, string otpCode, TimeSpan expiry);
        Task<string?> GetOtpAsync(string phoneNumber);
        Task DeleteOtpAsync(string phoneNumber);
        Task<int> GetOtpLimitCountAsync(string phoneNumber);
        Task IncrementOtpLimitAsync(string phoneNumber, TimeSpan expiry);

        // Temp User
        Task SetTempUserAsync(RegisterRequestDto user, TimeSpan expiry);
        Task<RegisterRequestDto?> GetTempUserAsync(string phoneNumber);
        Task DeleteTempUserAsync(string phoneNumber);
    }
}
