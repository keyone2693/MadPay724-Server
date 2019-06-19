using AutoMapper;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Presentation.Helpers
{
    public class AutoMapperProfiles : Profile
    {
       public AutoMapperProfiles()
        {
            CreateMap<User, UserFroListDto>();
            CreateMap<User, UserForDetailedDto>();
        }
    }
}
