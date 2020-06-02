﻿using AutoMapper;
using DatingApp.Dtos;
using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, op =>
                 op.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)).ForMember(dest => dest.Age, opt =>
                 opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailsDto>().ForMember(dest => dest.PhotoUrl, op =>
                 op.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)).ForMember(dest => dest.Age, opt => 
                 opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailDto>();

            CreateMap<User, UserForUpdateDto>().ReverseMap();
            CreateMap<Photo, PhotoToReturn>().ReverseMap();
            CreateMap<Photo, PhotoForCreationDto>().ReverseMap();
            CreateMap<UserForRegisterDto, User>().ReverseMap();
        }
    }
}
