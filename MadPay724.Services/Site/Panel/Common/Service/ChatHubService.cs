
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
    [Authorize(Policy = "RequireUserRole")]
    public class ChatHubService : Hub
    {
        private readonly UserInfoInMemory _userInfoInMemory;
        private readonly IHttpContextAccessor _http;

        public ChatHubService(UserInfoInMemory userInfoInMemory, IHttpContextAccessor http)
        {

            _http = http;
            _userInfoInMemory = userInfoInMemory;
        }

        public async Task LeaveAsync()
        {
            _userInfoInMemory.Remove(_http.HttpContext.User.Identity.Name);
            await Clients.AllExcept(new List<string> { _http.HttpContext.Connection.Id })
                .SendAsync("UserLeft", _http.HttpContext.User.Identity.Name);
        }
        public async Task JoinAsync()
        {
            if (!_userInfoInMemory.AddUpdate(_http.HttpContext.User.Identity.Name, _http.HttpContext.Connection.Id))
            {
                // new user

                var list = _userInfoInMemory.GetAllUsersExceptThis(_http.HttpContext.User.Identity.Name).ToList();
                await Clients.AllExcept(new List<string> { _http.HttpContext.Connection.Id })
                    .SendAsync("NeOnlineUser", _userInfoInMemory.GetUserInfo(_http.HttpContext.User.Identity.Name));
            }
            else
            {
                //existing user joined again
            }

            await Clients.Client(_http.HttpContext.Connection.Id)
                .SendAsync("Joined", _userInfoInMemory.GetUserInfo(_http.HttpContext.User.Identity.Name));

            await Clients.Client(_http.HttpContext.Connection.Id)
                .SendAsync("OnlineUsers", _userInfoInMemory.GetAllUsersExceptThis(_http.HttpContext.User.Identity.Name));
        }

        public Task SendDirectMessage(string message, string targetuserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(_http.HttpContext.User.Identity.Name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetuserName);

            return Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDirectMessage",message, userInfoSender);
        }
    }
}
