using Application.Services.BlockedUser;
using Application.Services.BlockedUser.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlockedUserController(IBlockedUserService _service) : ControllerBase
    {
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserDto model)
            => Ok(await _service.BlockUserAsync(GetUserId(), model));

        [HttpPost("unblock")]
        public async Task<IActionResult> UnBlockUser([FromBody] BlockUserDto model)
            => Ok(await _service.UnBlockUserAsync(GetUserId(), model));
        [HttpGet]
        public async Task<IActionResult> GetBlockedUsers()
            => Ok(await _service.GetBlockedUsersAsync(GetUserId()));

        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }
}
