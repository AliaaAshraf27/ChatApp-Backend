using Application.Services.Chats.Dtos;
using Application.Services.Contact;
using Application.Services.Messages.Dtos;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Contact.ContactName
{
    public class ContactNameService(IMemberRepository _memberRepo, IContactService _contactService, IChatRepository _chatRepo, IUserRepository _userRepo) : IContactNameService
    {
        public async Task<Dictionary<Guid, string>> GetDisplayNamesAsync(
        Guid currentUserId,
        IEnumerable<Guid> userIds)
        {
            var ids = userIds.Distinct().ToList();

            if (ids.Count == 0)
                return new Dictionary<Guid, string>();

            var userNames = await _userRepo.GetNamesByIdsAsync(ids);

            var contactNames =
                await _contactService.GetContactNamesAsync(
                    currentUserId, ids);

            var result = new Dictionary<Guid, string>();

            foreach (var id in ids)
            {
                if (!userNames.TryGetValue(id, out var userName))
                    continue;

                result[id] =
                    contactNames.TryGetValue(id, out var contactName)
                    && !string.IsNullOrWhiteSpace(contactName)
                        ? contactName
                        : userName;
            }

            return result;
        }
        public async Task ApplyContactNamesAsync(List<Chat> chats, Guid userId)
        {
            if (chats == null || chats.Count == 0)
                return;

            var otherMemberIds = await _memberRepo.GetPrivateChatOtherMemberIdsAsync(userId);
            if (otherMemberIds == null || otherMemberIds.Count == 0)
                return;

            var contactUserIds = chats
                .Where(c => otherMemberIds.ContainsKey(c.Id))
                .Select(c => otherMemberIds[c.Id])
                .Distinct()
                .ToList();

            if (contactUserIds.Count == 0)
                return;

            var contactNames = await _contactService.GetContactNamesAsync(userId, contactUserIds);
            if (contactNames == null || contactNames.Count == 0)
                return;

            foreach (var chat in chats)
            {
                if (otherMemberIds.TryGetValue(chat.Id, out var otherUserId) &&
                    contactNames.TryGetValue(otherUserId, out var contactName) &&
                    !string.IsNullOrWhiteSpace(contactName))
                {
                    chat.Name = contactName;
                }
            }
        }
        public async Task ApplySenderNamesAsync(IEnumerable<SenderDto> senders, Guid userId)
        {
            var list = senders.Where(s => s != null).ToList();
            if (list.Count == 0)
                return;

            var userIds = list.Select(s => s.Id).Distinct().ToList();
            var contactNames = await _contactService.GetContactNamesAsync(userId, userIds);
            if (contactNames == null || contactNames.Count == 0)
                return;

            foreach (var sender in list)
            {
                if (contactNames.TryGetValue(sender.Id, out var name) &&
                    !string.IsNullOrWhiteSpace(name))
                {
                    sender.Name = name;
                }
            }
        }
        //public async Task<List<Chat>> ApplyContactNamesAsync(Guid userId)
        //{
        //    var chats = await _chatRepo.GetAllPrivateChatsAsync(userId);
        //    if (chats == null || chats.Count == 0)
        //        return new List<Chat>();

        //    var otherMemberIds =
        //        await _memberRepo.GetPrivateChatOtherMemberIdsAsync(userId);

        //    if (otherMemberIds == null || otherMemberIds.Count == 0)
        //        return chats;

        //    var contactUserIds = chats
        //        .Where(c => c != null && otherMemberIds.ContainsKey(c.Id))
        //        .Select(c => otherMemberIds[c.Id])
        //        .Distinct()
        //        .ToList();

        //    if (contactUserIds.Count == 0)
        //        return chats;

        //    var contactNames = await _contactService.GetContactNamesAsync(
        //        userId,
        //        contactUserIds);

        //    if (contactNames == null || contactNames.Count == 0)
        //        return chats;

        //    foreach (var chat in chats)
        //    {
        //        if (chat == null)
        //            continue;

        //        if (otherMemberIds.TryGetValue(chat.Id, out var otherUserId) &&
        //            contactNames.TryGetValue(otherUserId, out var contactName) &&
        //            !string.IsNullOrWhiteSpace(contactName))
        //        {
        //            chat.Name = contactName;
        //        }
        //    }

        //    return chats;
        //}
    }
}
