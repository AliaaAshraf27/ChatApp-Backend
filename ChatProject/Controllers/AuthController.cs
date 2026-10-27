using Application.Response;
using Application.Services.Auth;
using Application.Services.Auth.Dtos;
using Application.Services.Otp.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        => Ok(await authService.SignUpAsync(request));

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
            => Ok(await authService.LoginAsync(request));

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
            => Ok(await authService.VerifyOtpAndGetTokenAsync(request));

        [Authorize]
        [HttpPut("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromForm] ProfileDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            return Ok( await authService.CompleteProfileAsync(model, Guid.Parse(userId)));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
            => Ok(await authService.RefreshTokenAsync(refreshToken));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
            => Ok(await authService.LogoutAsync(refreshToken));

    }
}
