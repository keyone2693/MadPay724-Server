using MadPay724.Common.OnlineChat.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadPay724.Common.OnlineChat.Storage
{
    public class UserInfoInMemory
    {
        private ConcurrentDictionary<string, UserInfo> _onlineUsers { get; set; } = new ConcurrentDictionary<string, UserInfo>();

        public bool AddUpdate(string name,string ConnectionId)
        {
            var userAlreadyExist = _onlineUsers.ContainsKey(name);

            var userInfo = new UserInfo
            {
                UserName = name,
                ConnectionId = ConnectionId
            };

            _onlineUsers.AddOrUpdate(name, userInfo, (key, value) => userInfo);

            return userAlreadyExist;
        }

        public void Remove(string name)
        {
            UserInfo userInfo;
            _onlineUsers.TryRemove(name, out userInfo);
        }

        public IEnumerable<UserInfo> GetAllUsersExceptThis(string userName)
        {
            return _onlineUsers.Values.Where(item => item.UserName != userName);
        }

        public UserInfo GetUserInfo(string userName)
        {
            UserInfo user;
            _onlineUsers.TryGetValue(userName,out user);
            return user;
        }
    }
}
