using Application.Helpers;
using Domain.IRepository;
using Application.Response;
using Application.Services.Auth.Dtos;
using Application.Services.Otp;
using Application.Services.Otp.Dto;
using AutoMapper;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Application.Services.Cache;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshRepository _tokenRepo;
        private readonly IOtpService _otpService;
        private readonly ICacheService _cache;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private static readonly TimeSpan TempUserExpiry = TimeSpan.FromMinutes(10);
        public AuthService(
            IUserRepository userRepo,
            IRefreshRepository tokenRepo,
            IOtpService otpService,
            IMapper mapper,
            IOptions<JWT> jwt,
            ICacheService cache)
        {
            _userRepo = userRepo;
            _tokenRepo = tokenRepo;
            _otpService = otpService;
            _cache = cache;
            _mapper = mapper;
            _jwt = jwt.Value;
        }
        public async Task<ApiResponse<string>> SignUpAsync(RegisterRequestDto model)
        {
            var existing = await _userRepo.GetByPhoneAsync(model.PhoneNumber);
            if (existing != null && existing.IsVerified)
                return ApiResponse<string>.FailureResponse("User already exists");

            if (existing != null && !existing.IsVerified)
                await _userRepo.DeleteAsync(existing);

            var otpResult = await _otpService.SendOtpAsync(model.PhoneNumber);
            if (!otpResult.Success)
                return ApiResponse<string>.FailureResponse(otpResult.Error!);

            await _cache.SetTempUserAsync(model, TempUserExpiry);
            return ApiResponse<string>.SuccessResponse(null, "OTP sent successfully");
        }
        public async Task<ApiResponse<string>> LoginAsync(LoginRequestDto model)
        {
            var user = await _userRepo.GetByPhoneAsync(model.PhoneNumber);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Invalid Phone Number");

            if (!user.IsVerified)
                return ApiResponse<string>.FailureResponse("Account is not verified. Please complete OTP verification first.");

            var otpResult = await _otpService.SendOtpAsync(model.PhoneNumber);
            if (!otpResult.Success)
                return ApiResponse<string>.FailureResponse(otpResult.Error);

            return ApiResponse<string>.SuccessResponse(null , "OTP sent, please verify");
        }
        public async Task<ApiResponse<AuthResponseDto>> VerifyOtpAndGetTokenAsync(VerifyOtpRequestDto model)
        {
            var result = await _otpService.VerifyOtpAsync(model);
            if (!result.Success)
                return ApiResponse<AuthResponseDto>.FailureResponse(result.Error!);

            var user = await _userRepo.GetByPhoneAsync(model.PhoneNumber);
            if (user == null)
            {
                var tempUser = await _cache.GetTempUserAsync(model.PhoneNumber);
                if (tempUser == null)
                    return ApiResponse<AuthResponseDto>.FailureResponse(
                        "Session expired, please register again");

                user = _mapper.Map<User>(tempUser);
                user.IsVerified = true;  
                user.IsOnline = true;
                await _userRepo.AddAsync(user);
                await _cache.DeleteTempUserAsync(model.PhoneNumber);
            }
            else
            {
                user.IsOnline = true;
                await _userRepo.UpdateAsync(user);
            }
            var response = await CreateAuthResponseAsync(user);
            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Authenticated");
        }
        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            var token = await _tokenRepo.GetActiveTokenAsync(refreshToken); 
            if (token == null)
                return ApiResponse<AuthResponseDto>.FailureResponse("Invalid refresh token");

            var user = await _userRepo.GetByIdAsync(token.UserId);
            if (user == null)
                return ApiResponse<AuthResponseDto>.FailureResponse("User not found");

            token.IsExpired = true;
            token.ExpiresAt = DateTime.UtcNow;
            await _tokenRepo.UpdateAsync(token);

            var response = await CreateAuthResponseAsync(user);
            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed");
        }
        public async Task<ApiResponse<string>> CompleteProfileAsync(ProfileDto model , Guid Id)
        {
            var user = await _userRepo.GetByIdAsync(Id);
            if (user == null)
                return ApiResponse<string>.FailureResponse("User not found");

            if (!user.IsVerified)
                return ApiResponse<string>.FailureResponse("Account not verified");

            _mapper.Map(model, user);
            await _userRepo.UpdateAsync(user);
            return ApiResponse<string>.SuccessResponse(null, "Profile completed");
           
        }
        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var token = await _tokenRepo.GetActiveTokenAsync(refreshToken);
            if (token == null)
                return ApiResponse<string>.FailureResponse("Invalid token");

            token.IsExpired = true;
            token.ExpiresAt = DateTime.UtcNow;
            await _tokenRepo.UpdateAsync(token);
            var user = await _userRepo.GetByIdAsync(token.UserId);
            if (user != null)
            {
                user.IsOnline = false;
                await _userRepo.UpdateAsync(user);
            }

            return ApiResponse<string>.SuccessResponse(null, "Logged out successfully");
        }

        #region Private methods
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())       
            };
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                 issuer: _jwt.Issuer,
                 audience: _jwt.Audience,
                 claims: claims,
                 expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                 signingCredentials: signingCredentials);

        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var token = await CreateJwtToken(user); 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<AuthResponseDto> CreateAuthResponseAsync(User user)
        {
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = await _tokenRepo.CreateAsync(user.Id);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }
            #endregion
        }
}
