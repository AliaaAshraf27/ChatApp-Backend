using Application.Services.Messages;
using Application.Services.Messages.Dtos;
using Application.Services.UserManagement.Dtos;
using Application.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/Message")]
    [ApiController]
    [Authorize]
    public class MessageController(IMessageService messageService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetChatMessages([FromQuery]MessageFilterDto model)
            => Ok(await messageService.GetChatMessagesAsync(model, GetUserId()));
        [HttpGet("{messageId}")]
        public async Task<IActionResult> GetById(Guid messageId)
            => Ok(await messageService.GetByIdAsync(messageId, GetUserId()));
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto model)
            => Ok(await messageService.SendMessageAsync(GetUserId(), model));
        [HttpPut]
        public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageDto model)
            => Ok(await messageService.UpdateMessageAsync(GetUserId(), model));
        [HttpDelete]
        public async Task<IActionResult> DeleteMessage([FromQuery] MessageActionDto model)
            => Ok(await messageService.DeleteMessageAsync(GetUserId(), model));
        [HttpPost("read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MessageActionDto model)
            => Ok(await messageService.MarkAsReadAsync(model, GetUserId()));
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] SearchMessageDto model)
           => Ok(await messageService.SearchAsync(model));
        [HttpPost("Forward")]
        public async Task<IActionResult> ForwardMessage([FromBody]ForwardMessageDto model)
            => Ok(await messageService.ForwardMessageAsync(GetUserId(), model));
        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }
}
