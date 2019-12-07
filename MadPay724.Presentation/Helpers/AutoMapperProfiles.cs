using AutoMapper;
using MadPay724.Common.Helpers;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Dtos.Site.Panel.EasyPay;
using MadPay724.Data.Dtos.Site.Panel.Photos;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models.MainDB;
using System.Collections.Generic;
using System.Linq;
using MadPay724.Data.Dtos.Site.Panel.Document;
using MadPay724.Data.Dtos.Site.Panel.Notification;
using MadPay724.Data.Dtos.Site.Panel.Ticket;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models.MainDB.UserModel;
using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Data.Models.MainDB.Blog;
using MadPay724.Data.Dtos.Site.Panel.BlogGroup;
using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Common.Helpers.Utilities.Extensions;

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
            CreateMap<User, UserForAccountantDto>()
                .ForMember(dest => dest.InventorySum, opt =>
                {
                    opt.MapFrom(src => src.Wallets.Sum(p => p.Inventory));
                })
                .ForMember(dest => dest.InterMoneySum, opt =>
                {
                    opt.MapFrom(src => src.Wallets.Sum(p => p.InterMoney));
                })
                .ForMember(dest => dest.ExitMoneySum, opt =>
                {
                    opt.MapFrom(src => src.Wallets.Sum(p => p.ExitMoney));
                })
                .ForMember(dest => dest.OnExitMoneySum, opt =>
                {
                    opt.MapFrom(src => src.Wallets.Sum(p => p.OnExitMoney));
                })
               .ForMember(dest => dest.PhotoUrl, opt =>
               {
                   opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
               })
               .ForMember(dest => dest.Age, opt =>
               {
                   opt.MapFrom(src => src.DateOfBirth.ToAge());
               });
            //*****
            CreateMap<Photo, PhotoForUserDetailedDto>();
            CreateMap<PhotoForProfileDto, Photo>();
            CreateMap<Photo, PhotoForReturnProfileDto>();
            CreateMap<BankCard, BankCardForUserDetailedDto>();
            CreateMap<UserForUpdateDto, User>();


            CreateMap<NotificationForUpdateDto, Notification>();

            CreateMap<BankCardForUpdateDto, BankCard>();
            CreateMap<BankCard,BankCardForReturnDto>();
            CreateMap<List<BankCardForUserDetailedDto>, List<BankCard>>();
            //CreateMap<List<BankCardForUserDetailedDto>, List<PagedList<BankCard>>>();
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
            CreateMap<EasyPay, EasyPayForReturnDto>();
            CreateMap<List<EasyPayForReturnDto>, List<EasyPay>>();
            CreateMap<EasyPayForCreateUpdateDto, EasyPay>();
            //----------------------------------------------------------------------
            CreateMap<BlogGroup, BlogGroupForReturnDto>();
            CreateMap<List<BlogGroupForReturnDto>, List<BlogGroup>>();
            CreateMap<BlogGroupForCreateUpdateDto, BlogGroup>();
            //----------------------------------------------------------------------
            CreateMap<BlogForCreateUpdateDto, Blog>();
            CreateMap<Blog, BlogForReturnDto>()
                .ForMember(dest => dest.UserName, opt =>
                {
                    opt.MapFrom(src => src.User.UserName);
                })
               .ForMember(dest => dest.Name, opt =>
               {
                   opt.MapFrom(src => src.User.Name);
               })
               .ForMember(dest => dest.BlogGroupName, opt =>
               {
                   opt.MapFrom(src => src.BlogGroup.Name);
               });
            CreateMap<List<BlogForReturnDto>, List<Blog>>();
            CreateMap<BlogForCreateUpdateDto, Blog>();
               
        }
    }
}
