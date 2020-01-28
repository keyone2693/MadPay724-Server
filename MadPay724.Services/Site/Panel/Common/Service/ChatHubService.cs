
using MadPay724.Common.OnlineChat.Storage;
using MadPay724.Services.Site.Panel.Common.Interface;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Panel.Common.Service
{
    public class ChatHubService : Hub
    {
        private readonly UserInfoInMemory _userInfoInMemory;
        private readonly string _name;
        private readonly string _connectionId;
        public ChatHubService(UserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
            _name = Context.User.Identity.Name;
            _connectionId = Context.ConnectionId;
        }

        public async Task LeaveAsync()
        {
            _userInfoInMemory.Remove(_name);
            await Clients.AllExcept(new List<string> { _connectionId })
                .SendAsync("UserLeft", _name);
        }
        public async Task JoinAsync()
        {
            if (!_userInfoInMemory.AddUpdate(_name, _connectionId))
            {
                // new user

                var list = _userInfoInMemory.GetAllUsersExceptThis(_name).ToList();
                await Clients.AllExcept(new List<string> { _connectionId })
                    .SendAsync("NeOnlineUser", _userInfoInMemory.GetUserInfo(_name));
            }
            else
            {
                //existing user joined again
            }

            await Clients.Client(_connectionId)
                .SendAsync("Joined", _userInfoInMemory.GetUserInfo(_name));

            await Clients.Client(_connectionId)
                .SendAsync("OnlineUsers", _userInfoInMemory.GetAllUsersExceptThis(_name));
        }

        public Task SendDirectMessage(string message, string targetuserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(_name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetuserName);

            return Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDirectMessage",message, userInfoSender);
        }
    }
}
