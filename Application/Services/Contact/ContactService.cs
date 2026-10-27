using Application.Response;
using Application.Services.Contact.Dtos;
using AutoMapper;
using Domain.IRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Services.UserManagement.Dtos;

namespace Application.Services.Contact
{
    public class ContactService(
        IContactRepository _contactRepo,
        IUserRepository _userRepo,
        IMapper _mapper) : IContactService
    {
        public async Task<ApiResponse<List<ContactDto>>> GetContactsAsync(Guid userId)
        {
            var contacts = await _contactRepo.GetContactsAsync(userId);
            if (contacts == null || !contacts.Any())
                return ApiResponse<List<ContactDto>>.FailureResponse("No contacts found");
            var result = _mapper.Map<List<ContactDto>>(contacts);
            return ApiResponse<List<ContactDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<bool>> AddContactAsync(Guid userId,CreateContactDto model)
        {
            if (userId == model.ContactUserId)
                return ApiResponse<bool>.FailureResponse("You cannot add yourself as a contact.");

            var contactUser = await _userRepo.GetByIdAsync(model.ContactUserId);
            if (contactUser == null)
                return ApiResponse<bool>.FailureResponse("User not found.");
            var exists = await _contactRepo.ExistsAsync(userId,model.ContactUserId);
            if (exists)
                return ApiResponse<bool>.FailureResponse("User is already in your contacts.");
          
            var contact = new Domain.Entities.Contact
            {
                UserId = userId,
                ContactUserId = model.ContactUserId,
                CreatedAt = DateTime.UtcNow,
                Name = string.IsNullOrWhiteSpace(model.Name)
                         ? contactUser.Name : model.Name,
            };
            await _contactRepo.AddAsync(contact);
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public async Task<ApiResponse<bool>> DeleteContactAsync(Guid userId,Guid contactUserId)
        {
            var exists = await _contactRepo.ExistsAsync(userId,contactUserId);
            if (!exists)
                return ApiResponse<bool>.FailureResponse("Contact not found.");
        
            await _contactRepo.DeleteAsync(userId,contactUserId);
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public Task<Dictionary<Guid, string>> GetContactNamesAsync(Guid userId,IEnumerable<Guid> contactUserIds)
               => _contactRepo.GetContactNamesAsync(userId, contactUserIds);
      
    }
}
