using Application.Services.BlockedUser.Dtos;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class BlockedUserProfile : Profile
    {
        public BlockedUserProfile()
        {
            CreateMap<BlockUserDto, UserBlock>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
