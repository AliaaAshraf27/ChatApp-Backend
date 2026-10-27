using Domain.IRepository;
using Application.Response;
using Application.Services.Otp.Dto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.Cache;
using System.Security.Cryptography;

namespace Application.Services.Otp
{
    public class OtpService(ICacheService _cache) : IOtpService
    {
        private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LimitExpiry = TimeSpan.FromMinutes(10);
        public async Task<ApiResponse<string>> SendOtpAsync(string phoneNumber)
        {
            var count = await _cache.GetOtpLimitCountAsync(phoneNumber);
            if (count >= 3)
                return ApiResponse<string>.FailureResponse(
                    "Too many OTP requests, please try again after 10 minutes");

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            await _cache.SetOtpAsync(phoneNumber, otpCode, OtpExpiry);
            await _cache.IncrementOtpLimitAsync(phoneNumber, LimitExpiry);

            return ApiResponse<string>.SuccessResponse(null, "OTP sent successfully");
        }
        public async Task<ApiResponse<bool>> VerifyOtpAsync(VerifyOtpRequestDto dto)
        { 
            var storedOtp = await _cache.GetOtpAsync(dto.PhoneNumber);
            if (storedOtp == null)
                return ApiResponse<bool>.FailureResponse(
                    "OTP expired or not found, please request a new one");

            if (storedOtp != dto.Code)
                return ApiResponse<bool>.FailureResponse("Invalid OTP code");

            await _cache.DeleteOtpAsync(dto.PhoneNumber);
            return ApiResponse<bool>.SuccessResponse(true, null);
        }


    }
}
