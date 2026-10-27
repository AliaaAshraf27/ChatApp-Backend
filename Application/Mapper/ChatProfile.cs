using Application.Services.Chats.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class ChatProfile : Profile
    {

        public ChatProfile()
        {
            CreateMap<Chat, ChatDto>()
              .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
              .ForMember(dest => dest.LastMessage,opt => opt.MapFrom(src =>src.Messages
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefault()))
              .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest) =>
                src.Image != null ? Convert.ToBase64String(src.Image) : null));
            
            CreateMap<CreatePrivateChatDto, Chat>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => ChatType.Private))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ChatMembers, opt => opt.Ignore());

            CreateMap<UpdateGroupChatDto, Chat>()
               .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest) =>
                    src.Image != null ? ConvertIFormFileToBytes(src.Image) : dest.Image))
               .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            
        }
        private static byte[]? ConvertIFormFileToBytes(IFormFile? file)
        {
            if (file == null) return null;
            using var dataStream = new MemoryStream();
            file.CopyTo(dataStream);
            return dataStream.ToArray();
        }
    }
}
