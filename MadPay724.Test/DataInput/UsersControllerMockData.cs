using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;

namespace MadPay724.Test.DataInput { 
    public static class UsersControllerMockData
{
        public static IEnumerable<User> GetUser()
        {
            var userList = new List<User>()
            {
                new User
                {
                    Id = "0d47394e-672f-4db7-898c-bfd8f32e2af7",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    DateOfBirth = DateTime.Now,
                    LastActive = DateTime.Now,
                    PasswordHash =new byte[255],
                    PasswordSalt = new byte[255],
                    UserName = "haysmathis@barkarama.com",
                    Name = "Holloway Vasquez",
                    PhoneNumber = "55",
                    Address = "55",
                    Gender = true,
                    City = "55",
                    IsAcive = true,
                    Status = true,
                    Photos = new List<Photo>()
                    {
                        new Photo()
                        {
                            Id = "0d47394e-672f-4db7-898c-bfd8f32e2af",
                            UserId = "0d47394e-672f-4db7-898c-bfd8f32e2af7",
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now,
                            PublicId = "1",
                            Url = "qq",
                            Alt = "qq",
                            IsMain = true,
                            Description = "qq",
                        }
                    }
                }
            };
            return userList;
        }

        public static UserForDetailedDto GetUserForDetailedDto()
        {
            return new UserForDetailedDto()
            {
                Id = "0d47394e-672f-4db7-898c-bfd8f32e2af7",
                UserName = "haysmathis@barkarama.com",
                Name = "Holloway Vasquez",
                PhoneNumber = "55",
                Address = "55",
                Gender = true,
                City = "55",
                Age = 15,
                LastActive = DateTime.Now,
                PhotoUrl = "qqq"
            };
        }
    }
}
