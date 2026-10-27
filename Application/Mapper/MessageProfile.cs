using Application.Services.ChatMembers.Dtos;
using Application.Services.Chats.Dtos;
using Application.Services.Messages.Dtos;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class MessageProfile : Profile
    {
        public MessageProfile() 
        {
            CreateMap<SendMessageDto, Message>()
               .ForMember(dest => dest.Id,opt => opt.MapFrom(_ => Guid.NewGuid()));

            CreateMap<Message, MessageDto>()
               .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender));
            CreateMap<Message, ResponseSearchDto>()
                .ForMember(d => d.SenderName,o => o.MapFrom(s => s.Sender.Name))
                .ForMember(d => d.ChatName,o => o.MapFrom(s => s.Chat.Name));

            CreateMap<Message, LastMessageDto>();
            CreateMap<ChatMember, MembersDto>()
               .ForMember(dest => dest.UserName,opt => opt.MapFrom(src => src.User.Name))
               .ForMember(dest => dest.Image,opt => opt.MapFrom(src =>
                    src.User.Image != null ? Convert.ToBase64String(src.User.Image) : null));

            CreateMap<User, SenderDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Image,opt =>
                    opt.MapFrom(src => src.Image != null ? Convert.ToBase64String(src.Image) : null));

            CreateMap<Message, Message>();
            CreateMap<Message, LastMessageDto>();
        }
    }
}
