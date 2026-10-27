using Application.Services.Status;
using Application.Services.Status.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusController(IStatusService _service) : ControllerBase
    {
        [HttpGet("contacts")]
        public async Task<IActionResult> GetStatuses()
            => Ok(await _service.GetContactsStatusesAsync(GetUserId()));
        [HttpGet("me")]
        public async Task<IActionResult> GetMyStatuses()
           => Ok(await _service.GetMyStatusesAsync(GetUserId()));

        [HttpPost]
        public async Task<IActionResult> CreateStatus([FromForm] CreateStatuesDto model)
            => Ok(await _service.CreateStatuesAsync(GetUserId(), model));
        [HttpPost("{statusId}/view")]
        public async Task<IActionResult> ViewStatus(Guid statusId)
            => Ok(await _service.ViewStatusAsync(GetUserId(), statusId));
        [HttpDelete("{statusId}")]
        public async Task<IActionResult> DeleteStatus(Guid statusId)
            => Ok(await _service.DeleteStatusAsync(GetUserId(), statusId));
        [HttpGet("{statusId}/viewers")]
        public async Task<IActionResult> GetViewers(Guid statusId)
            => Ok(await _service.GetViewersAsync(GetUserId(), statusId));

        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }

}
