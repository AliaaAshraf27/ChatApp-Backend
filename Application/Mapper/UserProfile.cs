using Application.Services.Auth.Dtos;
using Application.Services.Contact.Dtos;
using Application.Services.UserManagement.Dtos;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Image,
                   opt => opt.MapFrom(src => src.Image != null ? Convert.ToBase64String(src.Image) : null));

            CreateMap<User, SearchDto>()
                .ForMember(d => d.Image, o => o.MapFrom(s => 
                     s.Image == null ? null : Convert.ToBase64String(s.Image)));

            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Name)
                          ? src.ContactUser.Name: src.Name))
                .ForMember(dest => dest.PhoneNumber,
                            opt => opt.MapFrom(src => src.ContactUser.PhoneNumber))
                .ForMember(dest => dest.Image,
                   opt => opt.MapFrom(src => src.User.Image != null ? 
                              Convert.ToBase64String(src.User.Image) : null))
                .ForMember(dest => dest.Bio,opt => opt.MapFrom(src => src.ContactUser.Bio));
        }
    }
}
