using Application.Services.Contact;
using Application.Services.Contact.Dtos;
using Application.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController(IContactService _service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContacts()
            => Ok(await _service.GetContactsAsync(GetUserId()));
        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] CreateContactDto model)
            => Ok(await _service.AddContactAsync(GetUserId(), model));
        [HttpDelete("{contactUserId}")]
        public async Task<IActionResult> DeleteContact(Guid contactUserId)
            => Ok(await _service.DeleteContactAsync(GetUserId(), contactUserId));

        #region Helper method
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        #endregion
    }
}
