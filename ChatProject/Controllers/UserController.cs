using Application.Services.Auth.Dtos;
using Application.Services.Chats;
using Application.Services.UserManagement;
using Application.Services.UserManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
            => Ok(await userService.GetByIdAsync(GetUserId()));
        [HttpGet("{targetUserId}")]
        public async Task<IActionResult> GetUserProfile(Guid targetUserId)
           => Ok(await userService.GetUserProfileAsync(targetUserId, GetUserId()));
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileDto model)
            => Ok(await userService.UpdateProfileAsync(GetUserId(), model));
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] FilterDto model)
            => Ok(await userService.SearchAsync(model, GetUserId()));
        

        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }

}
