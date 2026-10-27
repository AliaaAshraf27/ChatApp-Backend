using Application.Services.ChatMembers;
using Application.Services.ChatMembers.Dtos;
using Application.Services.Chats;
using Application.Services.UserManagement.Dtos;
using Application.Services.UserManagement;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/Member")]
    [ApiController]
    [Authorize]
    public class MemberController(IMemberService memberService) : ControllerBase
    {
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMembers(Guid chatId)
           => Ok(await memberService.GetAllAsync(chatId, GetUserId()));
        [HttpPost("add")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberDto model)
           => Ok(await memberService.AddAsync(GetUserId(), model));
        
        [HttpPost("leaveGroup/{chatId}")]
        public async Task<IActionResult> LeaveGroup(Guid chatId)
           => Ok(await memberService.LeaveGroupAsync(chatId, GetUserId()));
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveMember([FromBody] MemberActionDto model)
        => Ok(await memberService.RemoveAsync(GetUserId(), model));
        [HttpPut("promote")]
        public async Task<IActionResult> Promote([FromBody] MemberActionDto model)
            => Ok(await memberService.PromoteAsync(model , GetUserId()));
        [HttpPut("demote")]
        public async Task<IActionResult> Demote([FromBody] MemberActionDto model)
            => Ok(await memberService.DemoteAsync(model, GetUserId()));
        [HttpPut("mute")]
        public async Task<IActionResult> Mute([FromBody] MuteChatDto model)
          => Ok(await memberService.MuteChatAsync(GetUserId(), model));
        [HttpPut("pin")]
        public async Task<IActionResult> Pin([FromBody] PinChatDto model)
          => Ok(await memberService.PinChatAsync(GetUserId(), model));
      
        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }
}
