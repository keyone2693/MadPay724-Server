using AutoMapper;
using MadPay724.Common.Helpers;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Dtos.Site.Panel.Photos;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Dtos.Common.ION;
using MadPay724.Data.Dtos.Common.Token;
using MadPay724.Data.Dtos.Site.Panel.Auth;
using MadPay724.Data.Dtos.Site.Panel.Document;
using MadPay724.Data.Dtos.Site.Panel.Notification;
using MadPay724.Data.Dtos.Site.Panel.Ticket;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models.UserModel;
using MadPay724.Data.Dtos.Site.Panel.Gate;

namespace MadPay724.Presentation.Helpers
{
    public class AutoMapperProfiles : Profile
    {
       public AutoMapperProfiles()
       {
           CreateMap<User, UserFroListDto>();
           //    .ForMember(dest => dest.Self, opt =>
           //        opt.MapFrom(src => 
           //        Link.To(nameof(Controllers.Site.V1.User.UsersController.GetUser),new { id= src.Id})))
           //    


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


            CreateMap<NotificationForUpdateDto, Notification>();

            CreateMap<BankCardForUpdateDto, BankCard>();
            CreateMap<BankCard,BankCardForReturnDto>();
            CreateMap<List<BankCardForUserDetailedDto>, List<BankCard>>();
CreateMap<BankCardForUpdateDto, BankCard>();

            CreateMap<List<DocumentForReturnDto>, List<Document>>();
            CreateMap<DocumentForCreateDto, Document>();
            CreateMap<Document, DocumentForReturnDto>();

            


            CreateMap<Wallet, WalletForReturnDto>();
            CreateMap<List<WalletForReturnDto>, List<BankCard>>();

            CreateMap<TicketForCreateDto, Ticket> ();

            //CreateMap<TokenResponseDto, LoginResponseDto>();

            //----------------------------------------------------------------------
            CreateMap<GateForCreateDto, Gate>();
            CreateMap<List<GateForReturnDto>, List<Gate>>();
            CreateMap<Gate, GateForReturnDto>();
            //----------------------------------------------------------------------
            CreateMap<EasyPayForCreateUpdateDto, EasyPay>();
            CreateMap<EasyPay, EasyPayForReturnDto>();
            CreateMap<List<EasyPayForReturnDto>, List<EasyPay>>();
            CreateMap<EasyPayForCreateUpdateDto, EasyPay>();
        }
    }
}
