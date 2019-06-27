using AutoMapper;
using MadPay724.Common.Helpers;
using MadPay724.Data.Dtos.Site.Admin.BankCards;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadPay724.Presentation.Helpers
{
    public class AutoMapperProfiles : Profile
    {
       public AutoMapperProfiles()
        {
            CreateMap<User, UserFroListDto>();
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
               {
                   opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
               })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom(src => src.DateOfBirth.ToAge());
                });
            CreateMap<Photo, PhotoForUserDetailedDto>();
            CreateMap<BankCard, BankCardForUserDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
        }
    }
}
