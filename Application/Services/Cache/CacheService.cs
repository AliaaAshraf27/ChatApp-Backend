using Application.Services.Auth.Dtos;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services.Cache
{
    public class CacheService(IConnectionMultiplexer _redis) : ICacheService
    {
        private readonly IDatabase _db = _redis.GetDatabase();
        #region OTp caching
        public async Task SetOtpAsync(string phoneNumber, string otpCode, TimeSpan expiry)
          => await _db.StringSetAsync($"otp:{phoneNumber}", otpCode, expiry);
        public async Task<string?> GetOtpAsync(string phoneNumber)
        {
            var value = await _db.StringGetAsync($"otp:{phoneNumber}");
            return value.HasValue ? value.ToString() : null;
        }
        public async Task DeleteOtpAsync(string phoneNumber)
         => await _db.KeyDeleteAsync($"otp:{phoneNumber}");
        public async Task<int> GetOtpLimitCountAsync(string phoneNumber)
        {
            var value = await _db.StringGetAsync($"otp_limit:{phoneNumber}");
            return value.HasValue ? int.Parse(value!) : 0;
        }
        public async Task IncrementOtpLimitAsync(string phoneNumber, TimeSpan expiry)
        {
            var key = $"otp_limit:{phoneNumber}";
            var count = await _db.StringIncrementAsync(key);

            // لو أول مرة بس حط الـ expiry
            if (count == 1)
                await _db.KeyExpireAsync(key, expiry);
        }
        #endregion

        #region User registration caching
        public async Task SetTempUserAsync(RegisterRequestDto user, TimeSpan expiry)
            => await _db.StringSetAsync($"temp_user:{user.PhoneNumber}",JsonSerializer.Serialize(user),expiry);
        public async Task<RegisterRequestDto?> GetTempUserAsync(string phoneNumber)
        {
            var value = await _db.StringGetAsync($"temp_user:{phoneNumber}");
            return value.HasValue? JsonSerializer.Deserialize<RegisterRequestDto>(value!): null;
        }
        public async Task DeleteTempUserAsync(string phoneNumber)
            => await _db.KeyDeleteAsync($"temp_user:{phoneNumber}");
     #endregion
    }
}

