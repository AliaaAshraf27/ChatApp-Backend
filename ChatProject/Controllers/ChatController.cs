using Application.Services.ChatMembers.Dtos;
using Application.Services.Chats;
using Application.Services.Chats.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/Chat")]
    [ApiController]
    [Authorize]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUserChats() 
            => Ok(await chatService.GetUserChatsAsync(GetUserId()));
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        => Ok(await chatService.GetByIdAsync(id, GetUserId()));
        [HttpPost("private")]
        public async Task<IActionResult> CreatePrivate([FromBody] CreatePrivateChatDto model)
        => Ok(await chatService.CreatePrivateAsync(GetUserId(), model));
        [HttpPost("group")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupChatDto model)
        => Ok(await chatService.CreateGroupAsync(GetUserId(), model));

        [HttpPut("group")]
        public async Task<IActionResult> UpdateGroup([FromForm] UpdateGroupChatDto model)
            => Ok(await chatService.UpdateGroupAsync(GetUserId(), model));

        [HttpDelete("private/{chatId}")]
        public async Task<IActionResult> DeletePrivate([FromRoute] Guid chatId)
            => Ok(await chatService.DeletePrivateAsync(chatId, GetUserId()));
        [HttpDelete("Group/{chatId}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid chatId)
          => Ok(await chatService.DeleteGroupAsync(chatId, GetUserId()));

        [HttpPost("archive/{chatId}")]
        public async Task<IActionResult> ArchiveChat([FromRoute] Guid chatId)
            => Ok(await chatService.ArchiveChatAsync(chatId, GetUserId()));
        [HttpGet("archive")]
        public async Task<IActionResult> GetArchivedChats()
            => Ok(await chatService.GetArchivedChatsAsync(GetUserId()));
        [HttpPost("pin/{chatId}")]
        public async Task<IActionResult> PinChat([FromRoute] Guid chatId)
            => Ok(await chatService.PinChatAsync(chatId, GetUserId()));
        [HttpGet("pin")]
        public async Task<IActionResult> GetPinnedChats()
            => Ok(await chatService.GetPinnedChatsAsync(GetUserId()));

        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }
}
