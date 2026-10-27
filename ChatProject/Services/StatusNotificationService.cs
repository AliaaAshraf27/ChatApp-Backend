using Api.SignalR;
using Application.Interfaces;
using Application.Services.Contact.ContactName;
using Application.Services.Status.Dtos;
using Microsoft.AspNetCore.SignalR;
namespace Api.Services
{

    public class StatusNotificationService(IHubContext<ChatHub> hub,IContactNameService nameService)
        : IStatusNotificationService
    {
        public async Task SendNewStatusAsync(Guid userId,IEnumerable<Guid> contactIds)
        {
            foreach (var contactId in contactIds.Distinct())
            {
                var names = await nameService.GetDisplayNamesAsync(contactId, new[] { userId });
                var userName = names.TryGetValue(userId, out var name)
                    ? name: "Unknown";

                await hub.Clients.User(contactId.ToString())
                    .SendAsync("NewStatus", new
                    {
                        UserId = userId,
                        UserName = userName,
                        Message = $"{userName} added a new status"
                    });
            }
        }
    }

}
