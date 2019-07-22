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
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Dtos.Common.ION;

namespace MadPay724.Presentation.Helpers
{
    public class AutoMapperProfiles : Profile
    {
       public AutoMapperProfiles()
       {
           CreateMap<User, UserFroListDto>()
               .ForMember(dest => dest.Self, opt =>
                   opt.MapFrom(src => 
                   Link.To(nameof(Controllers.V1.Site.Admin.UsersController.GetUser),new { id= src.Id})))
               ;


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
            CreateMap<PhotoForProfileDto, Photo>();
            CreateMap<Photo, PhotoForReturnProfileDto>();
            CreateMap<BankCard, BankCardForUserDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
        }
    }
}
