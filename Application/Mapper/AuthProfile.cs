using Application.Services.Auth.Dtos;
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
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<ProfileDto, User>()
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
