
using MadPay724.Services.Site.Panel.Common.Interface;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Panel.Common.Service
{
    public class ChatHubService : Hub<IChatClient>
    {
        public Task SendMessage(string user, string message)
        {
            return Clients.All.ReceiveMessage(user, message);
        }
        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.ReceiveMessage(message);
        }
    }
}
