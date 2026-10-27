using Application.Services.Status.Dtos;
using Application.Services.Status.Viewer;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class StatusProfile : Profile
    {
        public StatusProfile()
        {
            CreateMap<CreateStatuesDto, Status>();

            CreateMap<Status, UserStatusDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserImage,
                        opt => opt.MapFrom(src => src.User != null && src.User.Image != null
                          ? Convert.ToBase64String(src.User.Image) : null)); 

            CreateMap<Status, MyStatusDto>()
                .ForMember(dest => dest.UserName,opt => opt.MapFrom(src => "My Status"))
                .ForMember(dest => dest.UserImage,
                        opt => opt.MapFrom(src =>src.User != null && src.User.Image != null
                          ? Convert.ToBase64String(src.User.Image) : null));

            CreateMap<StatusView, StatusViewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Viewer.Name))
                .ForMember(dest => dest.Image,opt => opt.MapFrom(src => src.Viewer.Image != null ?
                              Convert.ToBase64String(src.Viewer.Image) : null));

        }
    }
}
