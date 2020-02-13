
using MadPay724.Common.OnlineChat.Storage;
using MadPay724.Services.Site.Panel.Common.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Panel.Common.Service
{
    [Authorize(Policy = "AccessChat")]
    public class ChatHubService : Hub
    {
        private readonly UserInfoInMemory _userInfoInMemory;

        public ChatHubService(UserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }

        public async Task Leave()
        {
            _userInfoInMemory.Remove(Context.User.Identity.Name);
            


            if (Context.User.Identity.Name == "admin@madpay724.com")
            {
                await Clients.AllExcept(new List<string> { Context.ConnectionId })
                .SendAsync("UserLeft", Context.User.Identity.Name);
            }
            else
            {
                var adminUser = _userInfoInMemory.GetUserInfo("admin@madpay724.com");

                await Clients.Client(adminUser.ConnectionId)
                .SendAsync("UserLeft", Context.User.Identity.Name);
            }

        }
        public async Task Join()
        {
            _userInfoInMemory.AddUpdate(Context.User.Identity.Name, Context.ConnectionId);
            
            if(Context.User.Identity.Name == "admin@madpay724.com")
            {
                await Clients.AllExcept(new List<string> { Context.ConnectionId })
                    .SendAsync("NewOnlineUser", _userInfoInMemory.GetUserInfo(Context.User.Identity.Name));
            }
            else {
                var adminUser = _userInfoInMemory.GetUserInfo("admin@madpay724.com");
                await Clients.Client(adminUser.ConnectionId)
                    .SendAsync("NewOnlineUser", _userInfoInMemory.GetUserInfo(Context.User.Identity.Name));
            }

            await Clients.Client(Context.ConnectionId)
                .SendAsync("Joined", _userInfoInMemory.GetUserInfo(Context.User.Identity.Name));

            //await Clients.Client(Context.ConnectionId)
            //    .SendAsync("OnlineUsers", _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name));
        }

        public Task SendDirectMessage(string message, string targetuserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(Context.User.Identity.Name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetuserName);

            return Clients.Client(userInfoReciever?.ConnectionId).SendAsync("SendDirectMessage",message, userInfoSender);
        }
    }
}
